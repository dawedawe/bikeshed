namespace BikeShed

module XmlViews =
    
    open Giraffe.XmlViewEngine

    let masterPage (pageTitle : string) (content : XmlNode list) =
        html [] [
            head [] [
                title [] [ encodedText pageTitle ]
                style [] [ rawText "label { display: inline-block; width: 80px; }" ]
            ]
            body [] [
                h1 [] [ encodedText pageTitle ]
                main [] content
             ]
        ]

    let newBikePage =
        [
            form [ attr "action" "/api/bikes"; attr "method" "POST" ] [
                div [] [
                    label [] [ rawText "Name:" ]
                    input [ attr "name" "Name"; attr "type" "text"; attr "placeholder" "name" ]
                ]
                div [] [
                    label [] [ rawText "Color:" ]
                    input [ attr "name" "Color"; attr "type" "text"; attr "placeholder" "value" ]
                ]
                input [ attr "type" "submit"; attr "value" "add new bike via xmlview" ]
            ]
        ] |> masterPage "New Bike"