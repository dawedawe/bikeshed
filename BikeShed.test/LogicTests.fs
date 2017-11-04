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
        createBike bike
        |> function
           | Success b -> Assert.Same(b.Name, bike.Name)
                          Assert.Same(b.Color, bike.Color)
           | Failure _ -> Assert.True(false)
        getBike bike.Name
        |> function
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
        let bike = { Name = "testbike2"; Color = "green" }
        createBike bike
        |> function
           | Success b -> Assert.Same(b.Name, bike.Name)
                          Assert.Same(b.Color, bike.Color)
           | Failure _ -> Assert.True(false)
        deleteBike bike.Name
        |> function
           | Success b -> Assert.Same(b, bike.Name)
           | Failure _ -> Assert.True(false)
        let bikes2 = getBikes()
        Assert.Equal(bikes1.Count, bikes2.Count)

    [<Fact>]
    let ``deleting unknown bike fails`` () =
        let bikes1 = getBikes()
        deleteBike "unknownbike"
        |> function
           | Success _ -> Assert.True(false)
           | Failure _ -> Assert.True(true)
        let bikes2 = getBikes()
        Assert.Equal(bikes1.Count, bikes2.Count)


    [<Fact>]
    let ``changing a bike works`` () =
        let bike = { Name = "testbike"; Color = "green" }
        createBike bike |> ignore
        let changedBike = { bike with Color = "red" }
        updateBike changedBike.Name changedBike
        |> function
           | Success b -> Assert.Same(b.Name, changedBike.Name)
                          Assert.Same(b.Color, changedBike.Color)
           | Failure _ -> Assert.True(false)


    [<Fact>]
    let ``changing a bikes name fails`` () =
        let bike = { Name = "testbike"; Color = "green" }
        createBike bike |> ignore
        let changedBike = { bike with Name = "changedname" }
        updateBike changedBike.Name changedBike
        |> function
           | Success _ -> Assert.True(false)
           | Failure _ -> Assert.True(true)

