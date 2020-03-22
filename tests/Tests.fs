module Tests

open Fable.Mocha
open App

let appTests = testList "App tests" [
    
]

let allTests = testList "All" [
    appTests
]

[<EntryPoint>]
let main (args: string[]) = Mocha.runTests allTests
