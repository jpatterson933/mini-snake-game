namespace SnakeGame.GameLogic;

public class Food
{
    public Position Location { get; private set; }
    public string Name { get; private set; }
    public string Color { get; private set; }

    private static readonly List<(string Name, string Color)> AvailableFoods = new()
    {
        ("Apple", "#ff6b9d"),
        ("Banana", "#ffe135"),
        ("Blueberry", "#4169e1"),
        ("Cherry", "#8b4513"),
        ("Grape", "#f3e5ab"),
        ("Lime", "#98ff98")
    };

    public Food(Position location)
    {
        Location = location;
        var randomFood = PickRandomFood();
        Name = randomFood.Name;
        Color = randomFood.Color;
    }

    private static (string Name, string Color) PickRandomFood()
    {
        var randomIndex = Random.Shared.Next(AvailableFoods.Count);
        return AvailableFoods[randomIndex];
    }

    public bool IsAtSameLocationAs(Position position) => Location.IsAtSameLocationAs(position);
}

