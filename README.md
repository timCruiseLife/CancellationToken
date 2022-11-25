# WebApi CancellationToken Example Project

此專案為個人練習 CancellationToken 與 EntityFrameworkCore 的使用<br>
This project is to practice CancellationToken and EntityFrameworkCore<br>

以留言板的CRUD為舉例<br>
Take the CRUD of the message board as an example<br>

資料庫為MySQL<br>
DB is MySQL<br>

DefaultConnection need to be edited<br>
appstting.json<br>
appsettings.Development.json<br>


執行以下command 更新 DB <br>
Execute command to update the DB<br>

```dotnet tool install --global dotnet-ef```<br>
<br>
```dotnet ef```<br>
<br>
```dotnet ef dbcontext scaffold "server=localhost;Port=3306;Database=example; User=<sql user>;Password=<sql password>;" "Pomelo.EntityFrameworkCore.MySql" -o ./Models -c ExamplesContext -f --project Example```
