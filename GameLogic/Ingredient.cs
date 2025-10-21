namespace SnakeGame.GameLogic;

public class Ingredient
{
    public Position Location { get; private set; }
    public string Name { get; private set; }
    public string Color { get; private set; }

    private static readonly List<(string Name, string Color)> AvailableIngredients = new()
    {
        ("Strawberry", "#ff6b9d"),
        ("Banana", "#ffe135"),
        ("Blueberry", "#4169e1"),
        ("Chocolate", "#8b4513"),
        ("Vanilla", "#f3e5ab"),
        ("Mint", "#98ff98")
    };

    public Ingredient(Position location)
    {
        Location = location;
        var randomIngredient = PickRandomIngredient();
        Name = randomIngredient.Name;
        Color = randomIngredient.Color;
    }

    private static (string Name, string Color) PickRandomIngredient()
    {
        var randomIndex = Random.Shared.Next(AvailableIngredients.Count);
        return AvailableIngredients[randomIndex];
    }

    public bool IsAtSameLocationAs(Position position) => Location.IsAtSameLocationAs(position);
}

