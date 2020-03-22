module App

open Feliz
open Feliz.Router
open Elmish

type State = {
    CurrentUrl: string list
    NavigationBarOpen : bool
}

type Msg =
    | UrlChanged of string list
    | ToggleNavigationBar

let init() = { CurrentUrl = Router.currentUrl(); NavigationBarOpen = false }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | UrlChanged nextUrl -> { state with CurrentUrl = nextUrl }, Cmd.none
    | ToggleNavigationBar -> { state with NavigationBarOpen = not state.NavigationBarOpen }, Cmd.none

type NavigationItemProps = {
    title: string
    icon: string
    titleVisible: bool
    url : string
}

let navigationItem = React.functionComponent(fun (props: NavigationItemProps) ->
    let (hovered, setHovered) = React.useState(false)
    let isActive = props.url  = Router.format(Array.ofList (Router.currentUrl()))

    Html.li [
        prop.onMouseEnter (fun _ -> setHovered(true))
        prop.onMouseLeave (fun _ -> setHovered(false))
        prop.style [ style.width (length.percent 100) ]

        prop.children [
            Html.div [
                prop.style [
                    style.borderLeftWidth 3
                    style.borderLeftStyle borderStyle.solid
                    style.transitionDurationMilliseconds Constants.transitionSpeed
                    if isActive then style.borderLeftColor Constants.navbarTextColor
                    else if hovered then style.borderLeftColor Constants.navbarHoverBackgroundColor
                    else style.borderLeftColor Constants.navbarBackgroundColor
                ]

                prop.children [
                    Html.a [
                        prop.href props.url
                        prop.style [
                            style.display.flex
                            style.alignItems.center
                            style.height (5 * Constants.fontSize)
                            style.textDecoration.none
                            style.transitionDurationMilliseconds Constants.transitionSpeed
                            style.color.white

                            if hovered || isActive then
                                style.color Constants.navbarTextColor
                                style.backgroundColor Constants.navbarHoverBackgroundColor
                                style.filter.grayscale 0
                                style.opacity 1.0
                            else
                                style.filter.grayscale 100
                                style.opacity 0.7
                        ]

                        prop.children [
                            Html.i [
                                prop.style [ style.margin(0, 24) ]
                                prop.className [ sprintf "fa fa-%s fa-2x" props.icon ]
                            ]

                            Html.span [
                                prop.style [ if not props.titleVisible then style.display.none ]
                                prop.text props.title
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ])

type NavigationBarOpenerProps = {
    toggleOpened : unit -> unit
    isOpen : bool
}

let navigationBarOpener = React.functionComponent(fun (props: NavigationBarOpenerProps) ->
    let (hovered, setHovered) = React.useState(false)

    Html.li [
        prop.onMouseEnter (fun _ -> setHovered(true))
        prop.onMouseLeave (fun _ -> setHovered(false))
        prop.style [ style.width (length.percent 100); style.marginTop length.auto ]
        prop.onClick (fun _ -> props.toggleOpened())
        prop.children [
            Html.div [
                prop.style [
                    style.borderLeftWidth 3
                    style.borderLeftStyle borderStyle.solid
                    style.transitionDurationMilliseconds Constants.transitionSpeed
                    if hovered then style.borderLeftColor Constants.navbarHoverBackgroundColor
                    else style.borderLeftColor Constants.navbarBackgroundColor
                ]

                prop.children [
                    Html.a [
                        prop.style [
                            style.display.flex
                            style.alignItems.center
                            style.height (5 * Constants.fontSize)
                            style.textDecoration.none
                            style.transitionDurationMilliseconds Constants.transitionSpeed
                            style.color.white

                            if hovered then
                                style.backgroundColor Constants.navbarHoverBackgroundColor
                                style.color Constants.navbarTextColor
                                style.filter.grayscale 0
                                style.opacity 1.0
                            else
                                style.filter.grayscale 100
                                style.opacity 0.7
                        ]

                        prop.children [
                            Html.i [
                                prop.style [ style.margin(0, length.auto) ]
                                prop.className [ if props.isOpen then "fa fa-chevron-left fa-2x" else "fa fa-chevron-right fa-2x"]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ])

type NavigationBarProps = {
    isOpen : bool
    hoverToOpen : bool
    onOpen : bool -> unit
}

let navigationBar = React.functionComponent(fun (props: NavigationBarProps) -> [
    let (hovered, setHovered) = React.useState(false)
    let navigationBarWidth = if hovered || props.isOpen then Constants.fullNavigationBarWidth else Constants.miniNavigationBarWidth
    Html.nav [
        prop.style [
            style.backgroundColor Constants.navbarBackgroundColor
            style.width navigationBarWidth
            style.height (length.vh(100))
            style.position.fixedRelativeToWindow
            style.transitionDurationMilliseconds Constants.transitionSpeed
            style.transitionTimingFunction.ease
        ]

        prop.onMouseEnter (fun _ -> if props.hoverToOpen then setHovered(true))
        prop.onMouseLeave (fun _ -> if props.hoverToOpen then setHovered(false))

        prop.children [
            Html.ul [
                prop.style [
                    style.custom("listStyle", "none")
                    style.padding 0
                    style.margin 0
                    style.display.flex
                    style.flexDirection.column
                    style.alignItems.center
                    style.height (length.vh(100))
                ]

                prop.children [
                    navigationItem {
                        title = "Home"
                        icon = "home"
                        url = Router.format "/"
                        titleVisible = hovered || props.isOpen
                    }

                    navigationItem {
                        title = "Contacts"
                        icon = "address-book"
                        url = Router.format "contacts"
                        titleVisible = hovered || props.isOpen
                    }

                    navigationItem {
                        title = "Settings"
                        icon = "cogs"
                        url = Router.format "settings"
                        titleVisible = hovered || props.isOpen
                    }

                    navigationBarOpener {
                        isOpen = props.isOpen
                        toggleOpened = fun _ -> props.onOpen(not props.isOpen)
                    }
                ]
            ]
        ]
    ]
])

let mainLayout (state: State) (dispatch: Msg -> unit) (xs: ReactElement list) =

    let mainLayoutPadding = if state.NavigationBarOpen then Constants.fullNavigationBarWidth else Constants.miniNavigationBarWidth
    React.fragment [
        navigationBar {
            isOpen = state.NavigationBarOpen
            onOpen = fun _ -> dispatch ToggleNavigationBar
            hoverToOpen = false
        }

        Html.main [
            prop.style [
                style.marginLeft mainLayoutPadding
                style.padding Constants.fontSize
                style.transitionDurationMilliseconds Constants.transitionSpeed
            ]

            prop.children xs
        ]
    ]

let render (state: State) (dispatch: Msg -> unit) =
    let activePage =
        match state.CurrentUrl with
        | [  ] ->
            React.fragment [
                Html.h1 "Navigation Bar with Feliz"
                Html.p "This example application demonstrates how to build a modern navigation bar in Feliz without any React or CSS frameworks. Instead, we are using the type-safe styling API in Feliz."
                Html.div [
                    Html.span "The application is based on "
                    Html.span [
                        Html.a [
                            prop.href "https://www.youtube.com/watch?v=biOMz4puGt8"
                            prop.text "Animated Responsive Navbar with CSS - Plus Other Useful Tricks"
                        ]
                    ]
                    Html.span " which uses only CSS to build the navigation bar."
                ]
            ]

        | [ "contacts" ] -> Html.h1 "Contacts"
        | [ "settings" ] -> Html.h1 "Settings"
        | _              -> Html.none

    mainLayout state dispatch  [
        Router.router [
            Router.onUrlChanged (UrlChanged >> dispatch)
            Router.application [ activePage ]
        ]
    ]