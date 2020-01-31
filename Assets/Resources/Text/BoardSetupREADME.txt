Story stages are built using text documents. Certain strings will tell the board loader what to load in next. There is a general order to keep though.


"Bubble Layout" is a sequence of numbers that denote colors of balls to spawn in the stage. If no numbers are written, the board spawn will be random (generally used for 
Versus type stages). N means an empty space. Don't forget the "E" placed at the end of the sequence to tell the board loader that we are done here.

Bubble Layout
1234561234561,
123456123456,
1234561234561,
123456123456,
E
----------------------------------
"HSM" is for Hamster Spawn Max. This should always be an even number between 6 and 12.

HSM
10
------------------------------------
"Special Hamster" sets up which special hamsters are available in this stage. "Y" means it is available, "N" means it is not. Dont' forget the "End" here as well.

Special Hamsters
Rainbow
Y
Dead
Y
Gravity
Y
Bomb
Y
End
------------------------------------
"AI" sets up the AI Players in this stage. The first number is it's name, which will determine it's sprites. Check the CHARACTERS enum (in Character.cs) for the number associated
with each character. The next number is the characters color. The last number is the difficulty. The final string is it's AI type, which will have special
personalities for different AI characters.

AI
3
1
2
Standard
-------------------------------------
"Mode" tells the loader which game mode this stage will be. Currently there are 3 modes: "Versus", "Clear", and "Points". Each have different settings.
- Versus is easy, just put "Versus" and you're good.
- Clear I guess just put a 0 on the second line?
- Points has 2 settings: The points required to win, and the numbers of throws allowed.

Mode
Versus

Mode
Clear
0

Mode
Points
2000
10
-------------------------------------
The last section is the "Board". Which simply is the exact name of the scene to load for this stage

Board
OneTube - Forest
-------------------------------------

Done