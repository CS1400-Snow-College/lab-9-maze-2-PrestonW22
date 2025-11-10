//preston waters, maze, 10/25/25
using System.ComponentModel;

Console.Title = "Maze Game";

        // Step 1: Display intro
        Console.WriteLine("Welcome to the Maze Game!");
        Console.WriteLine("Use the arrow keys to move. Find the '*' to win.");
        Console.WriteLine("Press any key to start...");
        Console.ReadKey(true);

        // Step 2: Load the maze from the file
        string[] mazeRows = File.ReadAllLines("maze.txt");

        // Clear screen and print maze
        Console.Clear();
        foreach (string row in mazeRows)
        {
            Console.WriteLine(row);
        }
        
       
        // enemy positions
        List<(int row, int col)> enemies = new List<(int row, int col)>();

    for (int row = 0; row < mazeRows.Length; row++)
{
    for (int col = 0; col < mazeRows[row].Length; col++)
    {
        if (mazeRows[row][col] == '%')
        {
            enemies.Add((row, col));
        }
    }
}
    
    
        // Track each enemy’s movement direction and step count
    List<int> enemyDirections = new List<int>();
    List<int> enemySteps = new List<int>();

     
     
        // Give every enemy a starting direction (down) and reset step count
    for (int i = 0; i < enemies.Count; i++)
    {
        enemyDirections.Add(1); // moving down
        enemySteps.Add(0);      // 0 steps taken yet
    }



        // Step 3: Basic user controls
        // Start position (top-left corner)
        int cursorTop = 1;
        int cursorLeft = 1;
        Console.SetCursorPosition(cursorLeft, cursorTop);
        
        // variables
        int charCollected = 0;
        Random rand = new Random();
        

        ConsoleKey key;
        bool running = true;
 do
        {
            key = Console.ReadKey(true).Key;

            int proposedTop = cursorTop;
            int proposedLeft = cursorLeft;

            // Process key input
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    proposedTop--;
                    break;
                case ConsoleKey.DownArrow:
                    proposedTop++;
                    break;
                case ConsoleKey.LeftArrow:
                    proposedLeft--;
                    break;
                case ConsoleKey.RightArrow:
                    proposedLeft++;
                    break;
                case ConsoleKey.Escape:
                    running = false;
                    break;
            }
    // Step 4: Try moving
    TryMove(proposedTop, proposedLeft, ref charCollected, mazeRows, ref cursorTop, ref cursorLeft);

    // Enemy movement
    EnemyMove(ref running, cursorTop, cursorLeft, mazeRows, enemies, enemyDirections, enemySteps);


            // Step 5: Check for win condition
            if (mazeRows[cursorTop][cursorLeft] == '#')
            {
                Console.Clear();
                Console.WriteLine(" yay you won! ");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);
                break;
            }

           
        } while (running);

    // Step 4: TryMove method
    static void TryMove(int proposedTop, int proposedLeft,  ref int charCollected, string[] mazeRows, ref int cursorTop, ref int cursorLeft)
{
    // Check bounds
    if (proposedTop < 0 || proposedTop >= mazeRows.Length)
        return;

    if (proposedLeft < 0 || proposedLeft >= mazeRows[proposedTop].Length)
        return;

    char nextChar = mazeRows[proposedTop][proposedLeft];


    // Check for walls
    if (nextChar == '*')
        return;

    if (nextChar == '|')
        return;

    // checks for keys
    if (nextChar == '^')
    {
        var rowChars = mazeRows[proposedTop].ToCharArray();
        rowChars[proposedLeft] = ' ';
        mazeRows[proposedTop] = new string(rowChars);

        // Visually remove it
        Console.SetCursorPosition(proposedLeft, proposedTop);
        Console.Write(' ');
        Console.SetCursorPosition(cursorLeft, cursorTop);

        charCollected++;
    }

    // opens door
    if (charCollected == 10)
    {
        for (int row = 0; row < mazeRows.Length; row++)
        {
            for (int col = 0; col < mazeRows[row].Length; col++)
            {
                if (mazeRows[row][col] == '|')
                {
                    // Replace the '|' with a space
                    var rowChars = mazeRows[row].ToCharArray();
                    rowChars[col] = ' ';
                    mazeRows[row] = new string(rowChars);

                    // Update it on screen immediately
                    Console.SetCursorPosition(col, row);
                    Console.Write(' ');
                }
            }
        }

    }
    // If valid move
    cursorTop = proposedTop;
    cursorLeft = proposedLeft;
    Console.SetCursorPosition(cursorLeft, cursorTop);
}

    static void EnemyMove( ref bool running, int cursorTop, int cursorLeft, string[] mazeRows, List<(int row, int col)> enemies, List<int> enemyDirections, List<int> enemySteps)

{
    // --- Move enemies up and down ---
    for (int i = 0; i < enemies.Count; i++)
    {
        int enemyRow = enemies[i].row;
        int enemyCol = enemies[i].col;
        int direction = enemyDirections[i];
        int steps = enemySteps[i];

        int newRow = enemyRow + direction;
        int newCol = enemyCol; // they only move vertically

        // Check bounds and walls
        if (newRow < 0 || newRow >= mazeRows.Length)
        {
            // Reverse direction if hitting maze edge
            enemyDirections[i] *= -1;
            continue;
        }

        char nextTile = mazeRows[newRow][newCol];

        // If next tile is a wall, reverse direction
        if (nextTile == '*' || nextTile == '|')
        {
            enemyDirections[i] *= -1;
            continue;
        }

        // Visually erase old position
        Console.SetCursorPosition(enemyCol, enemyRow);
        Console.Write(' ');

        // Visually draw new position
        Console.SetCursorPosition(newCol, newRow);
        Console.Write('%');

        // Update maze data
        var rowChars = mazeRows[enemyRow].ToCharArray();
        rowChars[enemyCol] = ' ';
        mazeRows[enemyRow] = new string(rowChars);

        rowChars = mazeRows[newRow].ToCharArray();
        rowChars[newCol] = '%';
        mazeRows[newRow] = new string(rowChars);

        // Update position
        enemies[i] = (newRow, newCol);

        // Count step and flip after 6
        enemySteps[i]++;
        if (enemySteps[i] >= 8)
        {
            enemySteps[i] = 0;
            enemyDirections[i] *= -1;
        }

        // Check collision with player
        if (newRow == cursorTop && newCol == cursorLeft)
        {
            Console.Clear();
            Console.WriteLine("Game Over! You were caught by an enemy!");
            Console.ReadKey(true);
            running = false;
            break;
        }
    }
}