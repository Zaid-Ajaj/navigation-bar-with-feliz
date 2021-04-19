module Main

open Feliz
open Browser
open Fable.Core.JsInterop

importAll "../styles/main.scss"

ReactDOM.render(App.Router(), document.getElementById "feliz-app")