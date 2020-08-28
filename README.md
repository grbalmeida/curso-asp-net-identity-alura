### Comando para instalação do ASP.NET Identity

```
Install-Package Microsoft.AspNet.Identity.Core -Version 2.2.1
```

### Comando para instalação do Entity Framework

```
Install-Package Microsoft.AspNet.Identity.EntityFramework -Version 2.2.1
```

### String de Conexão para o SQL Server

```xml
<connectionStrings>
    <add
		name="DefaultConnection"
		providerName="System.Data.SqlClient"
		connectionString="Data Source=(LocalDB)\MSSQLLocalDB;Database=ByteBank.Forum;trusted_connection=true"
    />
  </connectionStrings>
```