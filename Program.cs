DialogueHelper.Write("What size map do you wish to play on? (Small, Medium, Large) - ", ConsoleColor.Cyan);
TheFountainOfObjectsGame game = Console.ReadLine()?.ToLower() switch
{
    "small" => CreateSmallGame(),
    "medium" => CreateMediumGame(),
    "large" => CreateLargeGame()
};
DisplayNarrative();
game.Run();


//Display narrative
void DisplayNarrative()
{
    DialogueHelper.WriteLine("You enter The Cavern Of Objects, a maze of rooms filled with dangerous pits in search of the Fountain of Objects", ConsoleColor.Magenta);
    DialogueHelper.WriteLine("Light is visible only in the entrance, and no other light is seen anywhere in the caverns.", ConsoleColor.Magenta);
    DialogueHelper.WriteLine("You must navigate the Caverns with your other senses", ConsoleColor.Magenta);
    DialogueHelper.WriteLine("Find The Fountain Of Objects, activate it, and return to the entrance", ConsoleColor.Magenta);
    DialogueHelper.WriteLine("Look out for pits. You will feel a breeze if a pit is in an adjacent room. If you enter a room with a pit, you will die.", ConsoleColor.Magenta);
    DialogueHelper.WriteLine("Maelstroms are violent forces of sentient wind. Entering a room with one could transport you to any other location in the caverns.", ConsoleColor.Magenta);
    DialogueHelper.WriteLine("Amaroks roam the caverns. Encountering one is certain death, but you can smell their rotten stench in nearby rooms.", ConsoleColor.Magenta);
    DialogueHelper.WriteLine("You carry with you a bow and a quiver of arrows. You can use them to shoot monsters in the caverns but be warned. You have a limited supply.", ConsoleColor.Magenta);
}


// Creates a small game map
TheFountainOfObjectsGame CreateSmallGame()
{
    Map map = new Map(4, 4);
    Location startLocation = new Location(0, 0);
    Player player = new Player(startLocation);

    map.SetRoomType(startLocation, RoomType.CavernEntrance);
    map.SetRoomType(new Location(0, 2), RoomType.FountainRoom);
    map.SetRoomType(new Location(3, 3), RoomType.Pit);
    Monster[] monsters = new Monster[]
    {
        new Maelstrom(new Location(3, 0)),
        new Amarok(new Location(3, 2))
    };

    return new TheFountainOfObjectsGame(map, player, monsters);
}

// Creates a medium game map
TheFountainOfObjectsGame CreateMediumGame()
{
    Map map = new Map(6, 6);
    Location startLocation = new Location(0, 0);
    Player player = new Player(startLocation);

    map.SetRoomType(startLocation, RoomType.CavernEntrance);
    map.SetRoomType(new Location(5, 4), RoomType.FountainRoom);
    map.SetRoomType(new Location(4, 4), RoomType.Pit);
    map.SetRoomType(new Location(1, 1), RoomType.Pit);
    Monster[] monsters = new Monster[]
    {
        new Maelstrom(new Location(5, 0)),
        new Amarok(new Location(6, 5)),
        new Amarok(new Location(3, 3))
   
    };
    return new TheFountainOfObjectsGame(map, player, monsters);
}

// Creates a large game map
TheFountainOfObjectsGame CreateLargeGame()
{
    Map map = new Map(8, 8);
    Location startLocation = new Location(0, 0);
    Player player = new Player(startLocation);

    map.SetRoomType(startLocation, RoomType.CavernEntrance);
    map.SetRoomType(new Location(7, 4), RoomType.FountainRoom);
    map.SetRoomType(new Location(4, 4), RoomType.Pit);
    map.SetRoomType(new Location(1, 1), RoomType.Pit);
    map.SetRoomType(new Location(5, 7), RoomType.Pit);
    map.SetRoomType(new Location(3, 6), RoomType.Pit);
    Monster[] monsters = new Monster[]
    {
        new Maelstrom(new Location(3, 0)),
        new Maelstrom(new Location(7, 0)),
        new Amarok(new Location(7, 2)),
        new Amarok(new Location(0, 7)),
        new Amarok(new Location(6, 2))
    };

    return new TheFountainOfObjectsGame(map, player, monsters);
}

public class TheFountainOfObjectsGame
{
    public Map Map { get; }
    public Player Player { get; }
    private readonly ISense[] _senses;
    public Monster[] Monsters;
    public bool IsFountainActivated { get; set; }

    public TheFountainOfObjectsGame(Map map, Player player, Monster[] monsters)
    {
        Map = map;
        Player = player;
        Monsters = monsters;

        // New senses can be added to this list as the game is expanded upon, allowing for scalability
        _senses = new ISense[]
        {
            new CavernEntranceSense(),
            new FountainSense(),
            new PitSense(),
            new MaelstromSense(),
            new AmarokSense()
        };
    }

    // The main game loop
    public void Run()
    {

        // Game runs while the player hasn't completed the win conditions and still has life left
        while (!IsWon && Player.Health > 0)
        {
            DisplayGameStatus();
            ICommand playerCommand = GetCommand();
            playerCommand.Execute(this);

            foreach (Monster monster in Monsters)
            {
                if (monster.Location == Player.Location && monster.Health > 0) monster.Attack(this);
            }

            // Checks if the player has fallen down a pit
            if (Map.GetRoomType(Player.Location) == RoomType.Pit) Player.TakeDamage(3, "Falling down a pit!");


        }

        // Checks if the player has won and presents the relevant message
        if (IsWon)
        {
            DialogueHelper.WriteLine("The Fountain Of Objects has been reactivated, and you have escaped with your life!", ConsoleColor.Magenta);
            DialogueHelper.WriteLine("You Win!", ConsoleColor.Magenta);
        }
        // If the player has not won when the game has ended, present lost message
        else
        {
            DialogueHelper.WriteLine($"You died to: {Player.CauseOfDeath}", ConsoleColor.Red);
            DialogueHelper.WriteLine("You Lost!", ConsoleColor.Red);
        }
    }


    // Display the current status of the game, including the players location & any senses that may be triggered
    private void DisplayGameStatus()
    {
        DialogueHelper.WriteLine("---------------------------------------", ConsoleColor.White);
        DialogueHelper.WriteLine($"You are in the room at (Row={Player.Location.Row}, Column={Player.Location.Column})", ConsoleColor.White);
        DialogueHelper.WriteLine($"You have {Player.arrowCount} arrows remaining.", ConsoleColor.White);

        foreach (ISense sense in _senses)
        {
            if (sense.CanSense(this)) sense.Display(this);
        }
    }

    // Returns a ICommand object to get the players input
    private ICommand GetCommand()
    {
        // Keep looping until a valid command is entered
        while (true)
        {
            DialogueHelper.Write("What would you like to do? - ", ConsoleColor.Cyan);
            string? command = Console.ReadLine();

            if (command?.ToLower() == "move north") return new MoveCommand(Direction.North);
            if (command?.ToLower() == "move south") return new MoveCommand(Direction.South);
            if (command?.ToLower() == "move west") return new MoveCommand(Direction.West);
            if (command?.ToLower() == "move east") return new MoveCommand(Direction.East);
            if (command?.ToLower() == "enable fountain") return new ActivateCommand();
            if (command?.ToLower() == "shoot north") return new ShootCommand(Direction.North);
            if (command?.ToLower() == "shoot south") return new ShootCommand(Direction.South);
            if (command?.ToLower() == "shoot west") return new ShootCommand(Direction.West);
            if (command?.ToLower() == "shoot east") return new ShootCommand(Direction.East);
            if (command?.ToLower() == "help") return new HelpCommand();


            DialogueHelper.WriteLine($"{command} is not a valid not a valid command.", ConsoleColor.Red);
        }
    }

    // Checks whether the player has won or not dependent on if the fountain is activated and if the player is at the cave entrance
    public bool IsWon => IsFountainActivated && Map.GetRoomType(Player.Location) == RoomType.CavernEntrance;

}

// A class containing all information relevant to the player
public class Player
{
    // The players location within the game
    public Location Location { get; set; }

    // Used to store the players health count, although most deaths are instant, allows for monsters to only damage and not kill the player
    public int Health { get; private set; } = 3;

    // Used to store the players cause of death
    public string CauseOfDeath { get; private set; } = "";

    // An int to store the amount of arrows the players has
    public int arrowCount { get; set; } = 5;

    public Player(Location location)
    {
        Location = location;
    }

    public void TakeDamage(int damageCount, string damageSource)
    {
        Health = -damageCount;
        if (Health <= 0) CauseOfDeath = damageSource;
    }
}

// A record to store locations as
public record Location(int Row, int Column);

// Contains all information regarding to the map (Room information, map size etc.
public class Map
{
    private readonly RoomType[,] _rooms;
    public int Rows { get; set; }
    public int Columns { get; set; }

    public Map(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _rooms = new RoomType[Rows, Columns];
    }

    // Set the room type to a given room
    public void SetRoomType(Location location, RoomType room) => _rooms[location.Row, location.Column] = room;


    // Get the room type at the given location
    public RoomType GetRoomType(Location location) => RoomExists(location) ? _rooms[location.Row, location.Column] : RoomType.Empty;

    // Checks to see if the room exists on the map
    public bool RoomExists(Location location) => (location.Row < _rooms.GetLength(0) && location.Row >= 0 && location.Column < _rooms.GetLength(1) && location.Column >= 0);

    // Adjacency check for other rooms
    public bool IsAdjacent(Location location, RoomType roomType)
    {
        // Check vertical
        if (GetRoomType(new Location(location.Row + 1, location.Column)) == roomType) return true;
        if (GetRoomType(new Location(location.Row - 1, location.Column)) == roomType) return true;

        // Check horizontal
        if (GetRoomType(new Location(location.Row, location.Column + 1)) == roomType) return true;
        if (GetRoomType(new Location(location.Row, location.Column - 1)) == roomType) return true;

        // Check diagonal
        if (GetRoomType(new Location(location.Row + 1, location.Column + 1)) == roomType) return true;
        if (GetRoomType(new Location(location.Row - 1, location.Column + 1)) == roomType) return true;
        if (GetRoomType(new Location(location.Row + 1, location.Column - 1)) == roomType) return true;
        if (GetRoomType(new Location(location.Row - 1, location.Column - 1)) == roomType) return true;

        return false;
    }


}
// An interface for commands which further commands such as movement will be based off
public interface ICommand
{

    public void Execute(TheFountainOfObjectsGame game) { }
}


// A move command class based off the ICommand interface
public class MoveCommand : ICommand
{
    // The direction the player wants to move in
    public Direction Direction { get; }

    public MoveCommand(Direction direction)
    {
        Direction = direction;
    }

    // Executes the movement command
    public void Execute(TheFountainOfObjectsGame game)
    {

        Location currentPlayerLocation = game.Player.Location;
        Location newLocation = Direction switch
        {
            Direction.North => new Location(currentPlayerLocation.Row - 1, currentPlayerLocation.Column),
            Direction.South => new Location(currentPlayerLocation.Row + 1, currentPlayerLocation.Column),
            Direction.East => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column + 1),
            Direction.West => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column - 1)
        };

        if (game.Map.RoomExists(newLocation))
        {
            game.Player.Location = newLocation;
        }
        else
        {
            DialogueHelper.WriteLine("There is a wall here.", ConsoleColor.White);
        }

    }

}
// An activation command for the fountain based off ICommand
public class ActivateCommand : ICommand
{
    public void Execute(TheFountainOfObjectsGame game)
    {
        // Checks to see if the fountain is in the same room as player and responds accordingly
        if (game.Map.GetRoomType(game.Player.Location) == RoomType.FountainRoom) game.IsFountainActivated = true;
        else DialogueHelper.WriteLine("The fountain is not in this room.", ConsoleColor.White);
    }
}
// Command for firing arrows
public class ShootCommand : ICommand
{
    public Direction Direction { get; set; }
    public ShootCommand(Direction direction)
    {
        Direction = direction;
    }

    // Fires an arrow in the given direction
    public void Execute(TheFountainOfObjectsGame game)
    {
        // Check the players arrow count
        if (game.Player.arrowCount == 0)
        {
            DialogueHelper.WriteLine("You have no arrows remaining", ConsoleColor.Red);
            return;
        }

        // Gets the players location and then look at the location in the direction the player is aiming
        Location currentPlayerLocation = game.Player.Location;
        Location newLocation = Direction switch
        {
            Direction.North => new Location(currentPlayerLocation.Row - 1, currentPlayerLocation.Column),
            Direction.South => new Location(currentPlayerLocation.Row + 1, currentPlayerLocation.Column),
            Direction.East => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column + 1),
            Direction.West => new Location(currentPlayerLocation.Row, currentPlayerLocation.Column - 1)
        };

        // Tracks whether an arrow hit or not
        bool isTargetHit = false;

        // If the room exists, check through the monsters if any are there.
        if (game.Map.RoomExists(newLocation))
        {
            foreach(Monster monster in game.Monsters)
            {
                // If a monster does exist in the location and isn't dead, it takes damage.
                if(monster.Location == newLocation && monster.Health > 0)
                {
                    monster.TakeDamage(3);
                    isTargetHit = true;
                    DialogueHelper.WriteLine($"{monster} took an arrow straight to the heart and dies!", ConsoleColor.Magenta);
                }
            };
        };
        // If a target isn't hit, present a message
        if (!isTargetHit) DialogueHelper.WriteLine("Your arrow missed the target", ConsoleColor.Red);
        // Decrement the players arrow count
        game.Player.arrowCount--;
    }
}
// Displays help for the player when executed
public class HelpCommand: ICommand
{
    public void Execute(TheFountainOfObjectsGame game) 
    {
        DialogueHelper.WriteLine("help", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tDisplays information on available commands.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("enable fountain", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tEnables The Fountain Of Objects if you're in the room containing it. Required for the win condition.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("move north", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tWill move your character directly north from your current room unless there is a wall.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("move south", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tWill move your character directly south from your current room unless there is a wall.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("move east", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tWill move your character directly east from your current room unless there is a wall.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("move west", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tWill move your character directly west from your current room unless there is a wall.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("shoot north", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tFire an arrow into the room directly north of you. This will kill an enemy if one is in there and will cost you an arrow.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("shoot south", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tFire an arrow into the room directly south of you. This will kill an enemy if one is in there and will cost you an arrow.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("shoot east", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tFire an arrow into the room directly east of you. This will kill an enemy if one is in there and will cost you an arrow.", ConsoleColor.Gray);
        DialogueHelper.WriteLine("shoot west", ConsoleColor.Gray);
        DialogueHelper.WriteLine("\tFire an arrow into the room directly west of you. This will kill an enemy if one is in there and will cost you an arrow.", ConsoleColor.Gray);
    }
}

// Interface for sensing which individual senses will be based off
public interface ISense
{
    // Check to see if anything can be sensed
    public bool CanSense(TheFountainOfObjectsGame game);

    // Displays the appropriate message if CanSense returns true
    public void Display(TheFountainOfObjectsGame game);
}

// A sense for checking if the player is at the cavern entrance
public class CavernEntranceSense : ISense
{
    // Check if the cavern entrance can be sensed
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.GetRoomType(game.Player.Location) == RoomType.CavernEntrance;
    // Displays a message if it can be sensed
    public void Display(TheFountainOfObjectsGame game) => DialogueHelper.WriteLine("You see light in this room coming from outside the cavern. This is the entrance.",
                                                            ConsoleColor.Yellow);

}
// A sense for checking if the player can sense the fountain 
public class FountainSense : ISense
{
    // Checks if the fountain is in the player's room
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.GetRoomType(game.Player.Location) == RoomType.FountainRoom;

    // Displays an appropriate message if the fountain is on or off
    public void Display(TheFountainOfObjectsGame game)
    {
        if (!game.IsFountainActivated) DialogueHelper.WriteLine("You hear water dripping in this room. The Fountain Of Objects is here!", ConsoleColor.Blue);
        else DialogueHelper.WriteLine("You hear the rushing waters from The Fountain Of Objects. It has been reactivated!", ConsoleColor.Blue);
    }
}

// A sense for detecting pits
public class PitSense : ISense
{
    // Checks if a pit is nearby
    public bool CanSense(TheFountainOfObjectsGame game) => game.Map.IsAdjacent(game.Player.Location, RoomType.Pit);

    // Displays an appropriate warning
    public void Display(TheFountainOfObjectsGame game)
    {
        DialogueHelper.WriteLine("You feel a draft. There is a pit in a nearby room.", ConsoleColor.DarkGray);
    }
}
// A sense for detecting Maelstrom's
public class MaelstromSense : ISense
{
    // Checks for a nearby Maelstrom
    public bool CanSense(TheFountainOfObjectsGame game)
    {
        // Loop through each monster, getting only the Maelstroms and check for proximity to the players location
        foreach (Monster monster in game.Monsters)
        {
            if (monster is Maelstrom && monster.Health > 0)
            {
                int rowProximity = Math.Abs(monster.Location.Row - game.Player.Location.Row);
                int columnProximity = Math.Abs(monster.Location.Column - game.Player.Location.Column);
                if (rowProximity <= 1 && columnProximity <= 1) return true;
            }
        }
        return false;
    }

    public void Display(TheFountainOfObjectsGame game)
    {
        DialogueHelper.WriteLine("You hear the growling and groaning of a maelstrom nearby.", ConsoleColor.Gray);
    }
}
// A sense for detecting Amarok's
public class AmarokSense : ISense
{
    // Check for a nearby Amarok
    public bool CanSense(TheFountainOfObjectsGame game)
    {
        foreach(Monster monster in game.Monsters)
        {
            if (monster is Amarok amarok && monster.Health > 0)
            {
                int rowProximity = Math.Abs(monster.Location.Row - game.Player.Location.Row);
                int columnProximity = Math.Abs(monster.Location.Column - game.Player.Location.Column);
                if (rowProximity <= 1 && columnProximity <= 1) return true;
            }
        }
        return false;
    }

    public void Display(TheFountainOfObjectsGame game)
    {
        DialogueHelper.WriteLine("You can smell the rotten stench of an amarok in a nearby room.", ConsoleColor.Gray);
    }
    
}


// Abstract case which Monsters will be based off using polymorphism
public abstract class Monster
{
    // Monsters location
    public Location Location { get; set; }
    // Each monster will have 3 health, but could be expanded upon to add a challenge
    public int Health { get; set; } = 3;
    // Constructor for the monster
    public Monster(Location startLocation) => Location = startLocation;
    // Method to define each monsters ability to attack
    public abstract void Attack(TheFountainOfObjectsGame game);
    public abstract void TakeDamage(int damageAmount);
}

// Class for the Maelstrom variant of monster
public class Maelstrom : Monster
{
    // Constructor for maelstroms, using a location to start at
    public Maelstrom(Location startLocation) : base(startLocation) { }

    // A unique attack for the Maelstrom monster which displaces the player in the map
    public override void Attack(TheFountainOfObjectsGame game)
    {
        DialogueHelper.WriteLine("You have encountered a maelstrom! You have been blown away to somewhere else within the dungeon!", ConsoleColor.Magenta);
        game.Player.Location = WrapAround(new Location(game.Player.Location.Row - 1, game.Player.Location.Column + 2), game.Map.Rows, game.Map.Columns);
        Location = WrapAround(new Location(Location.Row + 1, Location.Column - 2), game.Map.Rows, game.Map.Columns);
    }

    // To account for the monster pushing the player off the map, a wrap around was used.
    public Location WrapAround(Location location, int mapRows, int mapColumns)
    {
        int row = location.Row;
        if (row < 0) row = mapRows + row;
        if (row >= mapRows) row = row - mapRows;

        int column = location.Column;
        if (column >= mapColumns) column = column - mapColumns;
        if (column < 0) column = mapColumns + column;

        return new Location(row, column);
    }
    public override void TakeDamage(int damageAmount)
    {
        Health =- damageAmount;
    }
}

// Amarok variant of monster
public class Amarok : Monster

{
    // Constructor for Amarok fed a starting location for the monster
    public Amarok(Location startLocation) : base(startLocation) { }

    // The Amarok's attack will kill the player
    public override void Attack(TheFountainOfObjectsGame game)
    {
        game.Player.TakeDamage(3, "Mauled by an Amarok");
    }
    public override void TakeDamage(int damageAmount)
    {
        Health = -damageAmount;
    }
}

// Static helper class which can be referenced for highlighting relevant messages within the game
public static class DialogueHelper
{
    // WriteLine method replacement for Console.WriteLine()
    public static void WriteLine(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.WriteLine(text);
    }

    // Write method replacement for Console.Write()
    public static void Write(string text, ConsoleColor colour)
    {
        Console.ForegroundColor = colour;
        Console.Write(text);
    }
}




// Enumeration to store the different types of room, allows for expanding upon
public enum RoomType { Empty, CavernEntrance, FountainRoom, Pit }

// Public enumeration to store a direction as it will be used in multiple instances
public enum Direction { North, South, East, West }