# XKDL (pronounced "Skuddle")

It's a variant of [KDL](https://kdl.dev/) (pronounced "Cuddle") for creating XAML files.

## todos

source generator
convert monkey details page
convert sample page

find default settings file
parse & load default settings

default properties

parse kdl
parse xkdl

colorize xkdl

indent tabs/spaces
indent size

override generated method signature

embed C# in XKDL

embed XAML in XKDL

determine default settings based on project type - walk to find the csproj file and read that to determine


## MAUI new

### original

```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiApp88.MainPage">

    <ScrollView>
        <VerticalStackLayout
            Padding="30,0"
            Spacing="25">
            <Image
                Source="dotnet_bot.png"
                HeightRequest="185"
                Aspect="AspectFit"
                SemanticProperties.Description="dot net bot in a race car number eight" />

            <Label
                Text="Hello, World!"
                Style="{StaticResource Headline}"
                SemanticProperties.HeadingLevel="Level1" />

            <Label
                Text="Welcome to &#10;.NET Multi-platform App UI"
                Style="{StaticResource SubHeadline}"
                SemanticProperties.HeadingLevel="Level2"
                SemanticProperties.Description="Welcome to dot net Multi platform App U I" />

            <Button
                x:Name="CounterBtn"
                Text="Click me" 
                SemanticProperties.Hint="Counts the number of times you click"
                Clicked="OnCounterClicked"
                HorizontalOptions="Fill" />
        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

### KXDL

```
ContentPage x:Class="MauiApp88.MainPage" {
    ScrollView {
        VerticalStackLayout Padding="30,0" Spacing="25" {
            Image Source="dotnet_bot.png" \
                  HeightRequest="185" \
                  Aspect="AspectFit" \
                  SemanticProperties.Description="dot net bot in a race car number eight"

            Label Text="Hello, World!" \
                  Style="{StaticResource Headline}" \
                  SemanticProperties.HeadingLevel="Level1"

            Label Text="Welcome to &#10;.NET Multi-platform App UI" \
                  Style="{StaticResource SubHeadline}" \
                  SemanticProperties.HeadingLevel="Level2" \
                  SemanticProperties.Description="Welcome to dot net Multi platform App U I"

            Button x:Name="CounterBtn" \
                  Text="Click me" \
                  SemanticProperties.Hint="Counts the number of times you click" \
                  Clicked="OnCounterClicked" \
                  HorizontalOptions="Fill"
        }
    }
}
```

#### XKDL 2

```
ContentPage {
    ScrollView {
        VerticalStackLayout Padding="30,0" /-Spacing="25" {
         
            Image Source="dotnet_bot.png" \
                  HeightRequest="185" \
                  Aspect="AspectFit" \
                  SemanticProperties.Description="dot net bot in a race car number eight"

            Label "Hello, World!" \
                  Style="{StaticResource Headline}" \
                  SemanticProperties.HeadingLevel="Level1"

            Label Text="Welcome to &#10;.NET Multi-platform App UI" \
                  Style="{StaticResource SubHeadline}" \
                  SemanticProperties.HeadingLevel="Level2" \
                  SemanticProperties.Description="Welcome to dot net Multi platform App U I"

            Button x:Name="CounterBtn" \
                  Text="Click me" \
                  SemanticProperties.Hint="Counts the number of times you click" \
                  Clicked="OnCounterClicked" \
                  HorizontalOptions="Fill" 

            Button Clicked="@count=0;@" x:Name="ResetBtn" Text="Reset counter"
        }
    }
}
```

### ENAMEL

```
ContentPage x:Class="MauiApp88.MainPage"

    ScrollView
        VerticalStackLayout Padding="30,0" Spacing="25"

            Image "dotnet_bot.png"
                  HeightRequest="185"
                  Aspect="AspectFit"
                  SemanticProperties.Description="dot net bot in a race car number eight"

            Headline "Hello, World!"

            SubHeadline
                "Welcome to &#10;.NET Multi-platform App UI"
                SemanticProperties.Description="Welcome to dot net Multi platform App U I"

            Button x:Name="CounterBtn"
                  Text="Click me"
                  SemanticProperties.Hint="Counts the number of times you click"
                  Clicked="OnCounterClicked"
```



## MAUI Monkey Finder

### original

```
<?xml version="1.0" encoding="utf-8" ?>
<c:VerticallyScrollingPage
    x:Class="MonkeyFinder.DetailsPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:c="clr-namespace:MonkeyFinder.Controls"
    xmlns:s="clr-namespace:MonkeyFinder"
    xmlns:str="clr-namespace:MonkeyFinder.Resources.Strings"
    xmlns:viewmodel="clr-namespace:MonkeyFinder.ViewModel"
    Title="{Binding Monkey.Name}"
    x:DataType="viewmodel:MonkeyDetailsViewModel">

    <c:DetailPageHeader Title="{Binding Monkey.Name}" ImageSource="{Binding Monkey.Image}" />

    <VerticalStackLayout x:Name="BodyContent" Spacing="{StaticResource InternalSpacing}">
        <s:StandardButton
            Margin="8"
            Command="{Binding OpenMapCommand}"
            HorizontalOptions="Center"
            Text="{x:Static str:UiText.ShowOnMapButtonContent}"
            WidthRequest="200" />

        <s:BodyText Text="{Binding Monkey.Details}" />
        <s:AdditionalInformation Text="{Binding Monkey.Location, StringFormat='Location: {0}'}" />
        <s:AdditionalInformation Text="{Binding Monkey.Population, StringFormat='Population: {0}'}" />
    </VerticalStackLayout>
</c:VerticallyScrollingPage>
```

### XKDL

```
c:VerticallyScrollingPage Title="{Monkey.Name}" \
                          x:Class="MonkeyFinder.DetailsPage" \
                          x:DataType="vm:MonkeyDetailsViewModel" {

    c:DetailPageHeader Title="{Monkey.Name}" \
                       ImageSource="{Monkey.Image}"

    VerticalStackLayout {

        StandardButton Command="{OpenMapCommand}" \
                       Text="{x:Static str:UiText.ShowOnMapButtonContent}"

        BodyText {Monkey.Details}
        s:AdditionalInformation {Monkey.FormattedLocation}
        s:AdditionalInformation {Monkey.FormattedPopulation}
    }
}
```

#### ENAMEL

```
c:VerticallyScrollingPage
    Title="{Monkey.Name}"
    x:Class="MonkeyFinder.DetailsPage"
    x:DataType="vm:MonkeyDetailsViewModel"

    c:DetailPageHeader
        Title="{Monkey.Name}"
        ImageSource="{Monkey.Image}"

    VerticalStackLayout

        StandardButton 
            Command="{OpenMapCommand}"
            Text="{x:Static str:UiText.ShowOnMapButtonContent}"

        BodyText {Monkey.Details}
        s:AdditionalInformation {Monkey.FormattedLocation}
        s:AdditionalInformation {Monkey.FormattedPopulation}
```
