namespace BikeShed

module Model =

    [<CLIMutable>]
    type Bike =
        {
            Name : string
            Color : string
        }

    
    [<CLIMutable>]
    type ColorQuery =
        {
            Color : string
        }