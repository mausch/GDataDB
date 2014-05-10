msbuild gdatadb.sln /m /t:rebuild /p:Configuration=Release
.nuget\nuget pack GDataDB.nuspec
.nuget\nuget pack GDataDB.Linq.nuspec