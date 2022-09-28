# DTOExampleSwithOutAllLayers

In this example we simply show using DTO and contracts (interfaces).

it is possible to seperate out all concerns to even switch from sql to mongo with just a 1 line change in di

```
       services.AddScoped<IAuthDataStoreContract, AuthDataStoreMongo>();
```

that is in the start up if you change it to
```
       services.AddScoped<IAuthDataStoreContract, AuthDataStoreSql>();
```

will swap it to use sql .. so you can play with both
