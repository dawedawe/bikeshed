namespace BikeShed

module Logic =

    open System.Collections.Generic
    open System.Linq
    open Model
    
    let dic = Dictionary<string, Bike>()

    do
        let bike1 = { Name = "Pinarello"; Color = "black"; }
        dic.Add(bike1.Name, bike1)
        
        let bike2 = { Name = "Giant"; Color = "blue"; }
        dic.Add(bike2.Name, bike2)
        
        let bike3 = { Name = "Canyon"; Color = "red"; }
        dic.Add(bike3.Name, bike3)
        
        let bike4 = { Name = "Focus"; Color = "white" }; 
        dic.Add(bike4.Name, bike4)
        
        let bike5 = { Name = "Radon"; Color = "white"; }
        dic.Add(bike5.Name, bike5)

    type Result<'TSuccess,'TFailure> = 
    | Success of 'TSuccess
    | Failure of 'TFailure

    let private bind (f : 'a -> Result<'b,'c>) : Result<'a,'c> -> Result<'b,'c> =
        fun result -> match result with
                      | Success o -> f o
                      | Failure e -> Failure e

    let private isValidNameOrColor s =
        let regex = System.Text.RegularExpressions.Regex("^[A-Z,a-z]+[A-Z,a-z,0-9,_]*$")
        regex.Match(s).Success

    let private bikes = 
        let dic = Dictionary<string, Bike>()
        let bike1 = { Name = "pinarello"; Color = "black"; }
        dic.Add(bike1.Name, bike1)
        let bike2 = { Name = "giant"; Color = "blue"; }
        dic.Add(bike2.Name, bike2)
        let bike3 = { Name = "canyon"; Color = "red"; }
        dic.Add(bike3.Name, bike3)
        dic

    let getBikes () = bikes.Values.ToList()

    let getBike bikeName =
        match bikes.TryGetValue(bikeName) with
        | true, bike -> Success bike
        | false, _ -> Failure "bike not found"

    let private validateBike bike =
        match isValidNameOrColor bike.Name && isValidNameOrColor bike.Color with
        | true -> Success bike
        | false -> Failure "bad bike definition"

    let private validateBikeUpdate bikeName bike =
        match bikeName = bike.Name with
        | true -> validateBike bike
        | false -> Failure "updating bikename not supported"

    let private createValidatedBike bike =
        if bikes.ContainsKey(bike.Name) then
            Failure (sprintf "bike %s already exists" bike.Name)
        else
            bikes.Add(bike.Name, bike)
            Success bike

    let createBike bike =
        bike
        |> validateBike
        |> bind createValidatedBike

    let deleteBike bikeName =
        match (bikes.Remove(bikeName)) with
        | true -> Success bikeName
        | false -> Failure (sprintf "bike %s not found" bikeName)

    let updateValidatedBike bike =
        if not (bikes.ContainsKey bike.Name) then
            Failure (sprintf "bike %s not found" bike.Name)
        else
            bikes.[bike.Name] <- bike
            Success bike


    let updateBike bikeName bike =
        validateBikeUpdate bikeName bike
        |> bind updateValidatedBike