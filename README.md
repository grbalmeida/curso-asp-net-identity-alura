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

### Comando para instalação do Owin

```
Install-Package Microsoft.Owin.Host.SystemWeb -Version 3.1.0
```

### Comando para instalação do pacote que faz a comunicação do Identity com o Owin

```
Install-Package Microsoft.AspNet.Identity.Owin -Version 2.2.1
```

### Comando para instalação do pacote de cookies do Owin

```
Install-Package Microsoft.Owin.Security.Cookies -Version 3.1.0
```