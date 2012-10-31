* Polish up existing pages.
* Design and implement moderator control panel and everything that goes with that, including point overrides for ineligible games.
* Implement the Steam API to link reddit and steam accounts.
* Add content to the site footer, sort of like how github uses their footer.
* Add various help text for various events. When a user is at the ratio limit, over the limit.
* When creating a PiF, can display a box on the right with helpful tips and answers to common questions.
* Warnings for when a ineligible user is selected, help tips when appropiate.
* Use the /me page as a design template to allow viewing of other users.
* Add statistics and other goodies (recommended games, an option for the site to randomly pick winners, recommended winners algorithims)
* Logging of different events, like when a user is over the ratio, Alt account detections.
* PiF Details page that displays everything related to the PiF, including comments and users who have made an entry to that PiF
* With the Steam API disallow entering for games owned or show a warning message, also can use the wishlist and library data for algorithims.
* On competion, site will need to edit the thread flair to reflect it's closed.
* On mod approval, site will need to assign the points and update the reddit flair.
* Mod action to override the point value for a game. Also need logging of these mod actions.
* With steam account linking and the site editing the flair, it's now possible to list the steam profile in the flair.
* Add/Edit game information via an admin only function.
* Implement a way for a user to specify a game that isn't in the site database, would usually only occur with games not on steam.
* Mod Queue for completed PiFs
* Ability to list and search for PiFs
* List all games given and received on a users page. Need also a public me.cshtml page.
* Once a winner is chosen, the site should make a comment reply to the winner's entry comment with an auth-req url to verify they received the game. Once all users have verifed they received the game, it's sent to the modqueue for approval. If possible, the site then should check the winners steam library to see if it's added to their library. Log and deny verified status until the game shows up in the library.
* A list of uncompleted PiFs that are X days old.
* Ability for mods to delete PiFs, probably just mark them as deleted, rather than actually deleting it. So users can't abuse creating PiFs and never fullfilling them. Or just log deletions and flag mods on certain abnormal events.