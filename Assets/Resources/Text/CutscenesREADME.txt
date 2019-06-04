Cutscenes are built using text documents. Special escape characters will tell the cutscene manager what to display next.

These are the escape characters:

T - Title of the cutscene

L - Location of the cutscene
*Essentially just loads the background image.

CL1 - Character that will say the next line while standing on left side
CL2 - Character that will say the next line while standing on left most side
CR1 - Character that will say the next line while standing on right side
CR2 - Character that will say the next line while standing on right most side
*For these, the next line is the character's name, and the line after that is their expression, and the line after that is the facing
*"Clear" instead of a characters name will move the character off screen.
*Facing text is not required, and should only be included if it is different from usual (i.e. character on the left side facing left)
*Facing will persist on a given character slot, so should be reset when changing characters
ex:
CL1
Boy
Happy
Left

D - Dialogue to be said, multiple lines can be done in a row.
*A line of dialogue has a maximum of 135 characters.*

S - Sound effect to play

B - Board to load
* If the board to load is set to "InGame" that means this cutscene is playing within a stage, so we should not load a new scene

E - Ends the cutscene
*Used for in-game cutscenes that don't need to load a new level at the end*

Done - This cutscene doesn't load into a level because it's at the end of a level so it will just load into the story select scene.