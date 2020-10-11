// Learn more about F# at http://fsharp.org

open System

let duplicateHead xs = seq { 
    yield Seq.head xs; 
    yield! xs 
    }

let (<||>) f g = (fun x -> f x || g x)

let GoBackDays (startDate: DateTime) (nDays:int) index =
    startDate.AddDays(float (index * -nDays));

let GetNextDay (startDate: DateTime) index =
    startDate.AddDays(float index)

let GetNextWeek (startDate: DateTime) index =
    startDate.AddDays(float (index * 7))

let GetNextMonth (startDate: DateTime) index =
    startDate.AddMonths(index)

let rec adjustDateForUnavailablity dateIsUnavailable (date: DateTime) =
    if dateIsUnavailable(date) then
        adjustDateForUnavailablity dateIsUnavailable (date.AddDays(float 1))
    else
        date

let isWeekend (date: DateTime) = 
    date.DayOfWeek = DayOfWeek.Saturday || date.DayOfWeek = DayOfWeek.Sunday

let isSpanishHoliday (date: DateTime) =
    date.Day = 12 && date.Month = 10

let isUSHoliday (date: DateTime) =
    date.Day = 13 && date.Month = 10

let isUKHoliday (date: DateTime) =
    date.Day = 14 && date.Month = 10

let BaseSchedule nextDateFn adjustDateFn =
    Seq.initInfinite nextDateFn
        |> Seq.map adjustDateFn
        |> Seq.distinct

let NoOutlierSchedule (nextDateFn: int -> DateTime) includeDateFn adjustDateFn =
    BaseSchedule nextDateFn adjustDateFn 
        |> Seq.takeWhile includeDateFn

let AllowOutlierSchedule (nextDateFn: int -> DateTime) includeDateFn adjustDateFn =
    let internalInclude (prevDate, _) =
        includeDateFn prevDate
    BaseSchedule nextDateFn adjustDateFn
        |> duplicateHead
        |> Seq.pairwise
        |> Seq.takeWhile internalInclude
        |> Seq.map (fun (_, next) -> next)

[<EntryPoint>]
let main argv =
    let startDate = new DateTime(2020, 10, 10);
    let endDate = new DateTime(2020, 10, 20);
    let dateFn = GetNextDay startDate
    let includeFn = (>=) endDate
    let isDateUnavailable = isWeekend <||> isUKHoliday <||> isSpanishHoliday <||> isUSHoliday
    let adjustDateFn = adjustDateForUnavailablity isDateUnavailable
    let seq = NoOutlierSchedule dateFn includeFn adjustDateFn

    for date in seq do
        Console.WriteLine date

    Console.WriteLine "------"
    Console.WriteLine "Reverse schedule ignore US holiday"

    let startDate2 = new DateTime(2020, 10, 10);
    let endDate2 = new DateTime(2020, 12, 20);
    
    let dateFn2 = GoBackDays endDate2 7
    let includeFn2 = (<=) startDate2
    let isDateUnavailable2 = isWeekend <||> isUKHoliday <||> isUSHoliday
    let adjustDateFn2 = adjustDateForUnavailablity isDateUnavailable2

    let seq2 = AllowOutlierSchedule dateFn2 includeFn2 adjustDateFn2

    for date in seq2 do
        Console.WriteLine date

    0 // return an integer exit code


