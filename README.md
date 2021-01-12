A tool to generate playable Ball Sort Puzzle level, with solution.

Example command:
.\BallSortGenerator.exe -levelCount 5 -levelOffset 0 -stackCount 10 -timeLimit 1 -solutionType first -path \levels

Launch options :

-levelCount x : Number of level generated.
		<Default> 1
		<Input type> Integer

-levelOffset x : Offset in level name. Level names will be level_[levelCount+levelOffset].bytes. 
		Example : with levelCount 5 and Offset 5 we will have 5 levels from 6 to 10.
		<Default> 0
		<Input type> Integer

-stackCount x : Total number of stacks required to win (not include 2 empty stacks)
		<Default> 4
		<Input type> Integer

-timeLimit x : Time (in seconds) limit to generate a single level. If it's fail to generate it will start another.
		<Default> 3
		<Input type> Float

-solutionType xxxx : only take "first" or "shortest" as option. 
		"first" :  generate level with first solution found.
		"shortest" generate level with the shortest solution.
		<Default> first
		<Input type> String

-path xxxxxxx : path to output files. Default to current folder
		<Default> ""
		<Input type> String