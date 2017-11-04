namespace BikeShed

module LogicTests =

    open System
    open Xunit
    open BikeShed.Model
    open BikeShed.Logic
    open System.Linq

    [<Fact>]
    let ``adding a bike works`` () =
        let bikes1 = getBikes()
        let bike = { Name = "testbike"; Color = "green" }
        match createBike bike with
        | Success b -> Assert.Same(b.Name, bike.Name)
                       Assert.Same(b.Color, bike.Color)
        | Failure _ -> Assert.True(false)
        let bikes2 = getBikes()
        Assert.True(bikes1.Where(fun b -> b.Name = bike.Name).Count() = 0)
        Assert.True(bikes2.Where(fun b -> b.Name = bike.Name).Count() = 1)
        Assert.Equal(bikes1.Count + 1, bikes2.Count)

    [<Fact>]
    let ``deleting a bike works`` () =
        let bikes1 = getBikes()
        let bike = { Name = "testbike"; Color = "green" }
        createBike bike |> ignore
        match deleteBike bike.Name with
        | Success b -> Assert.Same(b, bike.Name)
        | Failure _ -> Assert.True(false)
        let bikes2 = getBikes()
        Assert.Equal(bikes1.Count, bikes2.Count)