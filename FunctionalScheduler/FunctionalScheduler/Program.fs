// Learn more about F# at http://fsharp.org

open System
open System.Collections.Generic

module Seq =
    let doWhile predicate (s: seq<_>) = 
        let rec loop (en:IEnumerator<_>) = seq {
          if en.MoveNext() then
            yield en.Current
            if predicate en.Current then
              yield! loop en }

        seq { use en = s.GetEnumerator()
              yield! loop en }

    let infiniteSeq getNext start =
        seq {
            let mutable element = start
            while true do
                yield element
                element <- (getNext element)
        }

let (<||>) f g = (fun x -> f x || g x)

let GoForwardDays nDays (startDate: DateTime) =
    startDate.AddDays(float nDays)

let GoForwardWeeks nWeeks (startDate: DateTime)  =
    startDate.AddDays(float (nWeeks * 7))

let GoForwardMonths nMonths (startDate: DateTime) =
    startDate.AddMonths(nMonths)

let rec adjustInvalidValue isValueInvalid adjustValue value =
    if isValueInvalid value then
        let newValue = adjustValue value
        adjustInvalidValue isValueInvalid adjustValue newValue
    else
        value

let isWeekend (date: DateTime) = 
    date.DayOfWeek = DayOfWeek.Saturday || date.DayOfWeek = DayOfWeek.Sunday

let isSpanishHoliday (date: DateTime) =
    date.Day = 12 && date.Month = 10

let isUSHoliday (date: DateTime) =
    date.Day = 13 && date.Month = 10

let isUKHoliday (date: DateTime) =
    date.Day = 14 && date.Month = 10

[<EntryPoint>]
let main argv =
    let startDate = DateTime(2020, 10, 10);
    let endDate = DateTime(2020, 10, 20);

    let schedule = 
        Seq.infiniteSeq (GoForwardDays 1) startDate
            |> Seq.map (adjustInvalidValue (isWeekend <||> isUKHoliday <||> isSpanishHoliday <||> isUSHoliday) (GoForwardDays 1))
            |> Seq.distinct
            |> Seq.takeWhile ((>=) endDate)

    for date in schedule do
        Console.WriteLine date

    Console.WriteLine "------"
    Console.WriteLine "Reverse schedule ignore spanish holiday"

    let startDate2 = DateTime(2020, 12, 20); 
    let endDate2 = DateTime(2020, 10, 10);

    let schedule2 = 
        Seq.infiniteSeq (GoForwardWeeks -1) startDate2
            |> Seq.map (adjustInvalidValue (isWeekend <||> isUKHoliday <||> isUSHoliday) (GoForwardDays -1))
            |> Seq.distinct
            |> Seq.doWhile ((<=) endDate2)

    for date in schedule2 do
        Console.WriteLine date

    0 // return an integer exit code


