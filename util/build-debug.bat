@MSBuild /target:Build /property:Configuration=Debug ../MetaTweet.sln || (
	@ECHO Press ENTER key to exit.
	@PAUSE > NUL
)