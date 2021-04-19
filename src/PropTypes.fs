module PropTypes

type NavigationLinkProps = {
    title: string
    icon: string
    titleVisible: bool
    url : string
    active : bool
}

type NavigationBarOpenerProps = {
    toggleOpened : unit -> unit
    isOpen : bool
}

type NavigationBarProps = {
    isOpen : bool
    hoverToOpen : bool
    onOpen : bool -> unit
    currentUrl : string list
}