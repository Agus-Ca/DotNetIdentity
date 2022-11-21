# Comandos Code-First

| Comando PMC | Comando dotnet CLI | Uso |
|-------------|--------------------|-----|
| add-migration migrationName | Add migrationName | Crea una migracion haciendo un snapshot de la migracion. |
| Remove-migration | Remove | Removes the last migration snapshot. |
| Update-database | Update | Updates the database schema based on the last migration snapshot. |
| Script-migration | Script | Generates a SQL script using all the migration snapshots. |