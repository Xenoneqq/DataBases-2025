# Dokumentowe bazy danych – MongoDB

Ćwiczenie/zadanie


---

**Imiona i nazwiska autorów:**
Mateusz Pawliczek, Filip Malejki, Bartosz Ludwin

--- 

Odtwórz z backupu bazę north0

```
mongorestore --nsInclude='north0.*' ./dump/
```

```
use north0
```


# Zadanie 1 - operacje wyszukiwania danych,  przetwarzanie dokumentów

# a)

stwórz kolekcję  `OrdersInfo`  zawierającą następujące dane o zamówieniach
- pojedynczy dokument opisuje jedno zamówienie

```js
[  
  {  
    "_id": ...
    
    OrderID": ... numer zamówienia
    
    "Customer": {  ... podstawowe informacje o kliencie skladającym  
      "CustomerID": ... identyfikator klienta
      "CompanyName": ... nazwa klienta
      "City": ... miasto 
      "Country": ... kraj 
    },  
    
    "Employee": {  ... podstawowe informacje o pracowniku obsługującym zamówienie
      "EmployeeID": ... idntyfikator pracownika 
      "FirstName": ... imie   
      "LastName": ... nazwisko
      "Title": ... stanowisko  
     
    },  
    
    "Dates": {
       "OrderDate": ... data złożenia zamówienia
       "RequiredDate": data wymaganej realizacji
    }

    "Orderdetails": [  ... pozycje/szczegóły zamówienia - tablica takich pozycji 
      {  
        "UnitPrice": ... cena
        "Quantity": ... liczba sprzedanych jednostek towaru
        "Discount": ... zniżka  
        "Value": ... wartośc pozycji zamówienia
        "product": { ... podstawowe informacje o produkcie 
          "ProductID": ... identyfikator produktu  
          "ProductName": ... nazwa produktu 
          "QuantityPerUnit": ... opis/opakowannie
          "CategoryID": ... identyfikator kategorii do której należy produkt
          "CategoryName" ... nazwę tej kategorii
        },  
      },  
      ...   
    ],  

    "Freight": ... opłata za przesyłkę
    "OrderTotal"  ... sumaryczna wartosc sprzedanych produktów

    "Shipment" : {  ... informacja o wysyłce
        "Shipper": { ... podstawowe inf o przewoźniku 
           "ShipperID":  
            "CompanyName":
        }  
        ... inf o odbiorcy przesyłki
        "ShipName": ...
        "ShipAddress": ...
        "ShipCity": ... 
        "ShipCountry": ...
    } 
  } 
]  
```

## Rozwiązanie

Rozwiązanie tego zadania obejmowało znalezienie dla każdego rekordu z order informacji o pracownikach, klientach oraz zamówieniach oraz odpowiednie ich pogrupowanie i dodanie do nowo stworzonej kolekcji.

#### Inicjalizacja kolekcji

```mongodb
db.createCollection("OrdersInfo")
```

#### Wstawienie danych do kolekcji

```mongodb
db.orders.find().forEach(function(order) {
    const EmployeeData = db.employees.findOne({EmployeeID : order.EmployeeID});
    const CustomerData = db.customers.findOne({CustomerID : order.CustomerID})
    const Dates = {
        "OrderDate" : order.OrderDate,
        "RequiredDate" : order.RequiredDate,
    }
    const Freight = order.Freight
    let OrderTotal = 0;
    db.orderdetails.find({OrderID : order.OrderID}).forEach(function(orderdetail) {
        const value = (orderdetail.Quantity * orderdetail.UnitPrice) * (1- orderdetail.Discount);
        OrderTotal += value;
    });
    const Shipper = db.shippers.findOne({ShipperID : order.ShipVia})

    const Customer = {
        "CustomerID": CustomerData.CustomerID,
        "CompanyName": CustomerData.CompanyName,
        "City": CustomerData.City,
        "Country": CustomerData.Country,
    }

    const Employee = {
        "EmployeeID":EmployeeData.EmployeeID,
        "FirstName":EmployeeData.FirstName,
        "LastName":EmployeeData.LastName,
        "Title":EmployeeData.Title,
    }

    const OrderDetails = [];
    db.orderdetails.find({OrderID : order.OrderID}).forEach( function(orderdetail) {
        const productSearch = db.products.findOne({ProductID : orderdetail.ProductID})
        const category = db.categories.findOne({CategoryID : productSearch.CategoryID})
        const product = {
            "ProductID" : productSearch.ProductID,
            "ProductName" : productSearch.ProductName,
            "QuantityPerUnity" : productSearch.QuantityPerUnit,
            "CategoryID" : productSearch.CategoryID,
            "CategoryName" : category.CategoryName,
        }
        OrderDetails.push({
            "UnitPrice": orderdetail.UnitPrice,
            "Quantity": orderdetail.Quantity,
            "Discount": orderdetail.Discount,
            "Value": (orderdetail.UnitPrice * orderdetail.Quantity) - (1 - orderdetail.Discount),
            "product" : product,
        })
    });

    const Shipment = {
        "Shipper" : Shipper,
        "ShipName" : order.ShipName,
        "ShipAddress" : order.ShipAddress,
        "ShipCity" : order.ShipCity,
        "ShipCountry" : order.ShipCountry,
    }

    db.OrdersInfo.insertOne(
        {
            "OrderID":order.OrderID,

            "Customer" : Customer,

            "Employee" : Employee,

            "Dates" : Dates,

            "OrderDetails" : OrderDetails,

            "Freight" : Freight,
            "OrderTotal": OrderTotal,

            "Shipment" : Shipment,
        }
    )
})
```


# b)

stwórz kolekcję  `CustomerInfo`  zawierającą następujące dane kazdym klencie
- pojedynczy dokument opisuje jednego klienta

```js
[  
  {  
    "_id": ...
    
    "CustomerID": ... identyfikator klienta
    "CompanyName": ... nazwa klienta
    "City": ... miasto 
    "Country": ... kraj 

	"Orders": [ ... tablica zamówień klienta o strukturze takiej jak w punkcie a) (oczywiście bez informacji o kliencie)
	  
	]

		  
]  
```

## Rozwiązanie

#### Inicjalizacja kolekcji
```mongodb
db.createCollection("CustomerInfo")
```

#### Wstawienie danych do kolekcji

```mongodb
db.customers.find().forEach(function(customer) {
    const Orders = [];
    OrdersData = db.OrdersInfo.find({"Customer.CustomerID" : customer.CustomerID}).forEach(function (order) {
        Orders.push({
            "OrderID" : order.OrderID,
            "Employee" : order.Employee,
            "Dates" : order.Dates,
            "OrderDetails" : order.OrderDetails,
            "Freight" : order.Freight,
            "OrderTotal" : order.OrderTotal,
            "Shipment" : order.Shipment,
        })
    });

    db.CustomerInfo.insertOne({
        "CustomerID" : customer.CustomerID,
        "CompanyName" : customer.CompanyName,
        "City" : customer.City,
        "Country" : customer.Country,

        "Orders":Orders,
    })

})
```

# c) 

Napisz polecenie/zapytanie: Dla każdego klienta pokaż wartość zakupionych przez niego produktów z kategorii 'Confections'  w 1997r
- Spróbuj napisać to zapytanie wykorzystując
	- oryginalne kolekcje (`customers, orders, orderdertails, products, categories`)
	- kolekcję `OrderInfo`
	- kolekcję `CustomerInfo`

- porównaj zapytania/polecenia/wyniki

```js
[  
  {  
    "_id": 
    
    "CustomerID": ... identyfikator klienta
    "CompanyName": ... nazwa klienta
	"ConfectionsSale97": ... wartość zakupionych przez niego produktów z kategorii 'Confections'  w 1997r

  }		  
]  
```

## Rozwiązanie
### Używając oryginalnych kolekcji
```mongodb
db.customers.aggregate([
  // Dla każdego klienta łączymy zamówienia z 1997
  {
    $lookup: {
      from: "orders",
      let: { cid: "$CustomerID" },
      pipeline: [
        // filtrujemy po kliencie i dacie
        { $match: {
            $expr: {
              $and: [
                { $eq: ["$CustomerID", "$$cid"] },
                { $gte: ["$OrderDate", ISODate("1997-01-01T00:00:00Z")] },
                { $lt:  ["$OrderDate", ISODate("1998-01-01T00:00:00Z")] }
              ]
            }
        }},
        // dołączamy szczegóły pozycji
        { $lookup: {
            from: "orderdetails",
            localField: "OrderID",
            foreignField: "OrderID",
            as: "details"
        }},
        { $unwind: { path: "$details", preserveNullAndEmptyArrays: true }},

        // dołączamy dane produktu
        { $lookup: {
            from: "products",
            localField: "details.ProductID",
            foreignField: "ProductID",
            as: "product"
        }},
        { $unwind: { path: "$product", preserveNullAndEmptyArrays: true }},

        // dołączamy kategorię tylko jeśli to Confections
        { $lookup: {
            from: "categories",
            let: { catId: "$product.CategoryID" },
            pipeline: [
              { $match: {
                  $expr: {
                    $and: [
                      { $eq: ["$CategoryID", "$$catId"] },
                      { $eq: ["$CategoryName", "Confections"] }
                    ]
                  }
              }}
            ],
            as: "cat"
        }},
        { $unwind: { path: "$cat", preserveNullAndEmptyArrays: true }},

        // obliczamy wartość pozycji (0 jeśli nie Confections)
        { $project: {
            lineValue: {
              $cond: [
                { $gt: ["$cat", null] },
                { $multiply: ["$details.Quantity", "$details.UnitPrice", { $subtract: [1, "$details.Discount"] }] },
                0
              ]
            }
        }}
      ],
      as: "lines"
    }
  },

  // Sumujemy wszystkie wartości linii
  {
    $addFields: {
      totalConfections1997: { $sum: "$lines.lineValue" }
    }
  },

  // Końcowy projection i sortowanie
  {
    $project: {
      _id: 0,
      customerID: "$CustomerID",
      customerName: "$CompanyName",
      totalConfections1997: 1
    }
  },
  { $sort: { customerID: 1 } }
]);


```
### Używając OrdersInfo

```mongodb
db.OrdersInfo.aggregate([
  // Rozwiń każdą pozycję zamówienia
  { $unwind: "$OrderDetails" },

  // Dodaj pole lineValue
  {
    $addFields: {
      lineValue: {
        $cond: [
          {
            $and: [
              // data zamówienia w roku 1997
              { $gte: ["$Dates.OrderDate", ISODate("1997-01-01T00:00:00Z")] },
              { $lt:  ["$Dates.OrderDate", ISODate("1998-01-01T00:00:00Z")] },
              // produkt z kategorii Confections
              { $eq: ["$OrderDetails.product.CategoryName", "Confections"] }
            ]
          },
          // obliczamy Quantity * UnitPrice * (1 - Discount)
          {
            $multiply: [
              "$OrderDetails.Quantity",
              "$OrderDetails.UnitPrice",
              { $subtract: [1, "$OrderDetails.Discount"] }
            ]
          },
          // w przeciwnym razie
          0
        ]
      }
    }
  },

  // Grupuj po kliencie i sumuj lineValue
  {
    $group: {
      _id: {
        customerID:   "$Customer.CustomerID",
        customerName: "$Customer.CompanyName"
      },
      totalConfections1997: { $sum: "$lineValue" }
    }
  },

  // Formatujemy wynik
  {
    $project: {
      _id: 0,
      customerID:   "$_id.customerID",
      customerName: "$_id.customerName",
      totalConfections1997: 1
    }
  },

  // Sortowanie
  { $sort: { customerID: 1 } }
]);


```

### Używając CustomerInfo

```mongodb
db.CustomerInfo.aggregate([
  // Rozbijamy zamówienia na pojedyncze dokumenty
  { $unwind: {
      path: "$Orders",
      preserveNullAndEmptyArrays: true
  }},

  // Rozbijamy szczegóły zamówienia
  { $unwind: {
      path: "$Orders.OrderDetails",
      preserveNullAndEmptyArrays: true
  }},

  // 3. Dodajemy pole lineValue (tylko Confections z 1997)
  { $addFields: {
      lineValue: {
        $cond: [
          {
            $and: [
              // zamówienie w 1997
              { $gte: ["$Orders.Dates.OrderDate", ISODate("1997-01-01T00:00:00Z")] },
              { $lt:  ["$Orders.Dates.OrderDate", ISODate("1998-01-01T00:00:00Z")] },
              // produkt w kategorii Confections
              { $eq: ["$Orders.OrderDetails.product.CategoryName", "Confections"] }
            ]
          },
          // jeśli warunki spełnione → Quantity * UnitPrice * (1-Discount)
          { $multiply: ["$Orders.OrderDetails.Quantity", "$Orders.OrderDetails.UnitPrice", { $subtract: [1, "$Orders.OrderDetails.Discount"] }] },
          // w przeciwnym razie 0
          0
        ]
      }
  }},

  // Grupujemy po kliencie i sumujemy wszystkie lineValue
  { $group: {
      _id: {
        customerID:   "$CustomerID",
        customerName: "$CompanyName"
      },
      totalConfections1997: { $sum: "$lineValue" }
  }},

  // Formatujemy wynik
  { $project: {
      _id: 0,
      customerID:   "$_id.customerID",
      customerName: "$_id.customerName",
      totalConfections1997: 1
  }},

  // Sortujemy po ID klienta
  { $sort: { customerID: 1 } }
]);

```
*Uwaga: ilość wyników metodą z OrdersInfo różni sie od innych, ponieważ dwóch klientów nie składało żadnego zamówienia, więc nie ma ich w OrdersInfo.*
# d)

Napisz polecenie/zapytanie:  Dla każdego klienta poaje wartość sprzedaży z podziałem na lata i miesiące
Spróbuj napisać to zapytanie wykorzystując
	- oryginalne kolekcje (`customers, orders, orderdertails, products, categories`)
	- kolekcję `OrderInfo`
	- kolekcję `CustomerInfo`

- porównaj zapytania/polecenia/wyniki

```js
[  
  {  
    "_id": 
    
    "CustomerID": ... identyfikator klienta
    "CompanyName": ... nazwa klienta

	"Sale": [ ... tablica zawierająca inf o sprzedazy
	    {
            "Year":  ....
            "Month": ....
            "Total": ...	    
	    }
	    ...
	]
  }		  
]  
```

## Rozwiązanie
### Używając oryginalnych kolekcji
```mongodb
db.orders.aggregate([

  // Połącz z orderdetails i policz lineValue i wydziel rok/miesiąc
  { $lookup: {
      from:      "orderdetails",
      localField: "OrderID",
      foreignField:"OrderID",
      as:        "details"
    }
  },
  { $unwind: "$details" },
  { $project: {
      CustomerID: 1,
      lineValue: {
        $multiply: [
          "$details.Quantity",
          "$details.UnitPrice",
          { $subtract: [1, "$details.Discount"] }
        ]
      },
      year:  { $year:  "$OrderDate" },
      month: { $month: "$OrderDate" }
    }
  },

  //  Grupuj po CustomerID, roku i miesiącu
  { $group: {
      _id: {
        CustomerID: "$CustomerID",
        year:       "$year",
        month:      "$month"
      },
      totalSales: { $sum: "$lineValue" }
    }
  },

  // Dołącz nazwę klienta
  { $lookup: {
      from:         "customers",
      localField:   "_id.CustomerID",
      foreignField: "CustomerID",
      as:           "cust"
    }
  },
  { $unwind: "$cust" },

  // Zagnieżdż Sale w tablicy
  { $group: {
      _id:           "$_id.CustomerID",
      CustomerName:  { $first: "$cust.CompanyName" },
      Sale: {
        $push: {
          Year:  "$_id.year",
          Month: "$_id.month",
          Total: "$totalSales"
        }
      }
    }
  },

  // Projection i sortowanie
  { $project: {
      _id:         0,
      CustomerID:  "$_id",
      CompanyName: "$CustomerName",
      Sale:        1
    }
  },
  { $sort: { CustomerID: 1, Sale: 1 } }

]);
```

### Używając kolekcji OrdersInfo

```mongodb
db.OrdersInfo.aggregate([

  // Rozwiń każdą pozycję
  {
    $unwind: {
      path: "$Orderdetails",
      preserveNullAndEmptyArrays: true
    }
  },

  // Wyciągnij customer, lineValue, rok i miesiąc
  {
    $project: {
      CustomerID:   "$Customer.CustomerID",
      CompanyName:  "$Customer.CompanyName",
      lineValue:    "$OrderTotal",
      year:         { $year:  "$Dates.OrderDate" },
      month:        { $month: "$Dates.OrderDate" }
    }
  },

  // Grupuj po kliencie, roku i miesiącu
  {
    $group: {
      _id: {
        CustomerID:  "$CustomerID",
        CompanyName: "$CompanyName",
        year:        "$year",
        month:       "$month"
      },
      totalSales: { $sum: "$lineValue" }
    }
  },

  // Zagnieżdżamy tablicę Sale
  {
    $group: {
      _id: {
        CustomerID:  "$_id.CustomerID",
        CompanyName: "$_id.CompanyName"
      },
      Sale: {
        $push: {
          Year:  "$_id.year",
          Month: "$_id.month",
          Total: "$totalSales"
        }
      }
    }
  },

  // Projection i sortowanie
  {
    $project: {
      _id:         0,
      CustomerID:  "$_id.CustomerID",
      CompanyName: "$_id.CompanyName",
      Sale:        1
    }
  },
  { $sort: { CustomerID: 1, Sale: 1} }

]);
```

### Używając kolekcji CustomerInfo

```mongodb
db.CustomerInfo.aggregate([

  // Rozwiń tablicę Orders
  {
    $unwind: {
      path: "$Orders",
      preserveNullAndEmptyArrays: true
    }
  },

  // Wyciągamy potrzebne pola
  {
    $project: {
      CustomerID:   1,
      CompanyName:  1,
      lineValue:    "$Orders.OrderTotal",
      year:         { $year:  "$Orders.Dates.OrderDate" },
      month:        { $month: "$Orders.Dates.OrderDate" }
    }
  },

  // Grupujemy
  {
    $group: {
      _id: {
        CustomerID:  "$CustomerID",
        CompanyName: "$CompanyName",
        year:        "$year",
        month:       "$month"
      },
      totalSales: { $sum: "$lineValue" }
    }
  },

  // Zagnieżdżamy Sale
  {
    $group: {
      _id: {
        CustomerID:  "$_id.CustomerID",
        CompanyName: "$_id.CompanyName"
      },
      Sale: {
        $push: {
          Year:  "$_id.year",
          Month: "$_id.month",
          Total: "$totalSales"
        }
      }
    }
  },

  // Projection i sortowanie
  {
    $project: {
      _id:         0,
      CustomerID:  "$_id.CustomerID",
      CompanyName: "$_id.CompanyName",
      Sale:        1
    }
  },
  { $sort: { CustomerID: 1, Sale: 1 } }

]);
```
*Uwaga: w ostatnim zapytaniu jest o 2 wyniki więcej. Dzieje się tak ponieważ dwóch klientów nic nie zamawiało, więc nie zostali uwzględnieni w Orders czy OrdersInfo. W poleceniu z CustomerInfo wyświetlają się nastepująco:*
```json
[
  {
    "CompanyName": "Paris spécialités",
    "CustomerID": "PARIS",
    "Sale": [
      {
        "Year": null,
        "Month": null,
        "Total": 0
      }
    ]
  }
],

[
  {
    "CompanyName": "FISSA Fabrica Inter. Salchichas S.A.",
    "CustomerID": "FISSA",
    "Sale": [
      {
        "Year": null,
        "Month": null,
        "Total": 0
      }
    ]
  }
]
```

# e)

Załóżmy że pojawia się nowe zamówienie dla klienta 'ALFKI',  zawierające dwa produkty 'Chai' oraz "Ikura"
- pozostałe pola w zamówieniu (ceny, liczby sztuk prod, inf o przewoźniku itp. możesz uzupełnić wg własnego uznania)
Napisz polecenie które dodaje takie zamówienie do bazy
- aktualizując oryginalne kolekcje `orders`, `orderdetails`
- aktualizując kolekcję `OrderInfo`
- aktualizując kolekcję `CustomerInfo`

Napisz polecenie 
- aktualizując oryginalną kolekcję orderdetails`
- aktualizując kolekcję `OrderInfo`
- aktualizując kolekcję `CustomerInfo`

Załóżmy, że pojawia się nowe zamówienie dla klienta `ALFKI`, zawierające dwa produkty: `Chai` oraz `Ikura`. Pozostałe dane są uzupełnione przykładowo.

### Aktualizacja kolekcji `orders` i `orderdetails`

```javascript
const newOrderID = db.orders.find().sort({OrderID: -1}).limit(1).toArray()[0].OrderID + 1;
const customerData = db.customers.findOne({CustomerID: "ALFKI"});
const chaiProduct = db.products.findOne({ProductName: "Chai"});
const ikuraProduct = db.products.findOne({ProductName: "Ikura"});
const shipperID = 1;

db.orders.insertOne({
  OrderID: newOrderID,
  CustomerID: "ALFKI",
  EmployeeID: 5,
  OrderDate: new Date(),
  RequiredDate: new Date(new Date().setDate(new Date().getDate() + 7)),
  ShippedDate: null,
  ShipVia: shipperID,
  Freight: 15.50,
  ShipName: customerData.CompanyName,
  ShipAddress: customerData.Address,
  ShipCity: customerData.City,
  ShipRegion: customerData.Region,
  ShipPostalCode: customerData.PostalCode,
  ShipCountry: customerData.Country
});

db.orderdetails.insertMany([
  {
    OrderID: newOrderID,
    ProductID: chaiProduct.ProductID,
    UnitPrice: chaiProduct.UnitPrice,
    Quantity: 5,
    Discount: 0.05
  },
  {
    OrderID: newOrderID,
    ProductID: ikuraProduct.ProductID,
    UnitPrice: ikuraProduct.UnitPrice,
    Quantity: 3,
    Discount: 0.10
  }
]);
```

### Aktualizacja kolekcji `OrdersInfo`

```javascript
const newOrder = db.orders.findOne({OrderID: newOrderID});
const orderDetails = db.orderdetails.find({OrderID: newOrderID}).toArray();
const customer = db.customers.findOne({CustomerID: newOrder.CustomerID});
const employee = db.employees.findOne({EmployeeID: newOrder.EmployeeID});
const shipper = db.shippers.findOne({ShipperID: newOrder.ShipVia});

const orderDetailsInfo = [];
let orderTotal = 0;

orderDetails.forEach(function(detail) {
  const product = db.products.findOne({ProductID: detail.ProductID});
  const category = db.categories.findOne({CategoryID: product.CategoryID});
  const value = detail.UnitPrice * detail.Quantity * (1 - detail.Discount);
  orderTotal += value;

  orderDetailsInfo.push({
    UnitPrice: detail.UnitPrice,
    Quantity: detail.Quantity,
    Discount: detail.Discount,
    Value: value,
    product: {
      ProductID: product.ProductID,
      ProductName: product.ProductName,
      QuantityPerUnit: product.QuantityPerUnit,
      CategoryID: product.CategoryID,
      CategoryName: category.CategoryName
    }
  });
});

db.OrdersInfo.insertOne({
  OrderID: newOrder.OrderID,
  Customer: {
    CustomerID: customer.CustomerID,
    CompanyName: customer.CompanyName,
    City: customer.City,
    Country: customer.Country
  },
  Employee: {
    EmployeeID: employee.EmployeeID,
    FirstName: employee.FirstName,
    LastName: employee.LastName,
    Title: employee.Title
  },
  Dates: {
    OrderDate: newOrder.OrderDate,
    RequiredDate: newOrder.RequiredDate
  },
  OrderDetails: orderDetailsInfo,
  Freight: newOrder.Freight,
  OrderTotal: orderTotal,
  Shipment: {
    Shipper: {
      ShipperID: shipper.ShipperID,
      CompanyName: shipper.CompanyName
    },
    ShipName: newOrder.ShipName,
    ShipAddress: newOrder.ShipAddress,
    ShipCity: newOrder.ShipCity,
    ShipCountry: newOrder.ShipCountry
  }
});
```

### Aktualizacja kolekcji `CustomerInfo`

```javascript
const newOrderInfo = db.OrdersInfo.findOne({OrderID: newOrderID});
const orderForCustomer = {
  OrderID: newOrderInfo.OrderID,
  Employee: newOrderInfo.Employee,
  Dates: newOrderInfo.Dates,
  OrderDetails: newOrderInfo.OrderDetails,
  Freight: newOrderInfo.Freight,
  OrderTotal: newOrderInfo.OrderTotal,
  Shipment: newOrderInfo.Shipment
};

db.CustomerInfo.updateOne(
  { CustomerID: "ALFKI" },
  { $push: { Orders: orderForCustomer } }
);
```

---

# f)

### Aktualizacja `orderdetails`

```javascript
const currentDetails = db.orderdetails.find({OrderID: newOrderID}).toArray();

currentDetails.forEach(function(detail) {
  const newDiscount = detail.Discount + 0.05;
  db.orderdetails.updateOne(
    { OrderID: newOrderID, ProductID: detail.ProductID },
    { $set: { Discount: newDiscount } }
  );
});
```

### Aktualizacja `OrdersInfo`

```javascript
const orderInfo = db.OrdersInfo.findOne({OrderID: newOrderID});
let newOrderTotal = 0;

const updatedOrderDetails = orderInfo.OrderDetails.map(detail => {
  const newDiscount = detail.Discount + 0.05;
  const newValue = detail.UnitPrice * detail.Quantity * (1 - newDiscount);
  newOrderTotal += newValue;

  return {
    ...detail,
    Discount: newDiscount,
    Value: newValue
  };
});

db.OrdersInfo.updateOne(
  { OrderID: newOrderID },
  { 
    $set: { 
      OrderDetails: updatedOrderDetails,
      OrderTotal: newOrderTotal
    } 
  }
);
```

### Aktualizacja `CustomerInfo`

```javascript
const updatedOrderInfo = db.OrdersInfo.findOne({OrderID: newOrderID});

db.CustomerInfo.updateOne(
  { CustomerID: "ALFKI", "Orders.OrderID": newOrderID },
  { 
    $set: { 
      "Orders.$.OrderDetails": updatedOrderInfo.OrderDetails,
      "Orders.$.OrderTotal": updatedOrderInfo.OrderTotal
    } 
  }
);
```


Napisz polecenie które modyfikuje zamówienie dodane w pkt e)  zwiększając zniżkę  o 5% (dla każdej pozycji tego zamówienia) 

Napisz polecenie 
- aktualizując oryginalną kolekcję `orderdetails`
- aktualizując kolekcję `OrderInfo`
- aktualizując kolekcję `CustomerInfo`



UWAGA:
W raporcie należy zamieścić kod poleceń oraz uzyskany rezultat, np wynik  polecenia `db.kolekcka.fimd().limit(2)` lub jego fragment


# Zadanie 2 - modelowanie danych


Zaproponuj strukturę bazy danych dla wybranego/przykładowego zagadnienia/problemu

Należy wybrać jedno zagadnienie/problem (A lub B lub C)

Przykład A
- Wykładowcy, przedmioty, studenci, oceny
	- Wykładowcy prowadzą zajęcia z poszczególnych przedmiotów
	- Studenci uczęszczają na zajęcia
	- Wykładowcy wystawiają oceny studentom
	- Studenci oceniają zajęcia

Przykład B
- Firmy, wycieczki, osoby
	- Firmy organizują wycieczki
	- Osoby rezerwują miejsca/wykupują bilety
	- Osoby oceniają wycieczki

Przykład C
- Własny przykład o podobnym stopniu złożoności

a) Zaproponuj  różne warianty struktury bazy danych i dokumentów w poszczególnych kolekcjach oraz przeprowadzić dyskusję każdego wariantu (wskazać wady i zalety każdego z wariantów)
- zdefiniuj schemat/reguły walidacji danych
- wykorzystaj referencje
- dokumenty zagnieżdżone
- tablice

b) Kolekcje należy wypełnić przykładowymi danymi

c) W kontekście zaprezentowania wad/zalet należy zaprezentować kilka przykładów/zapytań/operacji oraz dla których dedykowany jest dany wariant

W sprawozdaniu należy zamieścić przykładowe dokumenty w formacie JSON ( pkt a) i b)), oraz kod zapytań/operacji (pkt c)), wraz z odpowiednim komentarzem opisującym strukturę dokumentów oraz polecenia ilustrujące wykonanie przykładowych operacji na danych

Do sprawozdania należy kompletny zrzut wykonanych/przygotowanych baz danych (taki zrzut można wykonać np. za pomocą poleceń `mongoexport`, `mongdump` …) oraz plik z kodem operacji/zapytań w wersji źródłowej (np. plik .js, np. plik .md ), załącznik powinien mieć format zip

## Zadanie 2  - rozwiązanie

---
## Tworzenie DBs: (to będzie do wywalenia)
#### Wariant 1
```mongoDB
// 1. Przełączamy się na bazę
use auctionDB1;

// 2. Kolekcja users
db.createCollection("users", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["username","email","passwordHash"],
      properties: {
        username:   { bsonType: "string", description: "nazwa użytkownika, obowiązkowe" },
        email:      { bsonType: "string", pattern:   "^.+@.+\\..+$", description: "poprawny e-mail" },
        passwordHash:{bsonType:"string", description:"hash hasła"},
        refreshTokens: {
          bsonType: "array",
          items: {
            bsonType: "object",
            required: ["token","issuedAt"],
            properties: {
              token:    { bsonType: "string" },
              issuedAt: { bsonType: "date" }
            }
          }
        }
      }
    }
  }
});

// 3. Kolekcja auction (meta dane aukcji)
db.createCollection("auction", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["title","startDate","endDate","status"],
      properties: {
        title:       { bsonType: "string" },
        category:    { bsonType: "string" },
        description: { bsonType: "string" },
        media: {
          bsonType: "array",
          items: { bsonType: "string", pattern: "^https?://" }
        },
        startDate:   { bsonType: "date" },
        endDate:     { bsonType: "date" },
        status:      { enum: ["active","closed","scheduled"] }
      }
    }
  }
});

// 4. Kolekcja auction.bids
db.createCollection("auction.bids", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["auctionId","userId","amount","placedAt"],
      properties: {
        auctionId: { bsonType: "objectId" },
        userId:    { bsonType: "objectId" },
        amount:    { bsonType: "double", minimum: 0 },
        placedAt:  { bsonType: "date" }
      }
    }
  }
});

// 5. Kolekcja auction.history
db.createCollection("auction.history", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["auctionId","winnerUserId","finalPrice","bids","closedAt"],
      properties: {
        auctionId:    { bsonType: "objectId" },
        winnerUserId: { bsonType: "objectId" },
        finalPrice:   { bsonType: "double", minimum: 0 },
        bids: {
          bsonType: "array",
          items: { bsonType: "objectId" }
        },
        closedAt:     { bsonType: "date" }
      }
    }
  }
});

// 6. Kolekcja log
db.createCollection("log", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["entity","entityId","action","timestamp"],
      properties: {
        entity:    { enum: ["user","auction","bid","history"] },
        entityId:  { bsonType: "objectId" },
        action:    { bsonType: "string" },
        timestamp: { bsonType: "date" },
        meta:      { bsonType: "object" }
      }
    }
  }
});
```
#### Wariant 2
```mongoDB
// 1. Przełączamy się na bazę
use auctionDB2;

// 2. Kolekcja users
db.createCollection("users", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["username","email","passwordHash"],
      properties: {
        username:   { bsonType: "string" },
        email:      { bsonType: "string", pattern: "^.+@.+\\..+$" },
        passwordHash:{bsonType:"string"}
      }
    }
  }
});

// 3. Kolekcja auction (ze zagnieżdżonymi bids)
db.createCollection("auction", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["title","startDate","endDate","status","bids"],
      properties: {
        title:       { bsonType: "string" },
        category:    { bsonType: "string" },
        description: { bsonType: "string" },
        media: {
          bsonType: "array",
          items: { bsonType: "string", pattern: "^https?://" }
        },
        startDate:   { bsonType: "date" },
        endDate:     { bsonType: "date" },
        status:      { enum: ["active","closed","scheduled"] },
        bids: {
          bsonType: "array",
          items: {
            bsonType: "object",
            required: ["userId","amount","placedAt"],
            properties: {
              userId:   { bsonType: "objectId" },
              amount:   { bsonType: "double", minimum: 0 },
              placedAt: { bsonType: "date" }
            }
          }
        }
      }
    }
  }
});

// 4. Kolekcja auction.history (z embedowanymi bidami)
db.createCollection("auction.history", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["auctionId","bids","closedAt","winnerUserId"],
      properties: {
        auctionId:    { bsonType: "objectId" },
        title:        { bsonType: "string" },
        bids: {
          bsonType: "array",
          items: {
            bsonType: "object",
            required: ["userId","amount","placedAt"],
            properties: {
              userId:   { bsonType: "objectId" },
              amount:   { bsonType: "double" },
              placedAt: { bsonType: "date" }
            }
          }
        },
        closedAt:     { bsonType: "date" },
        winnerUserId: { bsonType: "objectId" },
        status:       { enum: ["active","closed","scheduled"] }
      }
    }
  }
});

// 5. Kolekcja log
db.createCollection("log", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["action","timestamp"],
      properties: {
        action:    { bsonType: "string" },
        timestamp: { bsonType: "date" },
        meta:      { bsonType: "object" }
      }
    }
  }
});
```
#### Wariant 3:
```mongoDB
// 1. Przełączamy się na bazę
use auctionDB3;

// 2. Kolekcja users
db.createCollection("users", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["username","email","passwordHash"],
      properties: {
        username:    { bsonType: "string" },
        email:       { bsonType: "string", pattern: "^.+@.+\\..+$" },
        passwordHash:{ bsonType: "string" }
      }
    }
  }
});

// 3. Kolekcja auction (meta + bidsPreview)
db.createCollection("auction", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["title","startDate","endDate","status","bidsPreview"],
      properties: {
        title:       { bsonType: "string" },
        category:    { bsonType: "string" },
        description: { bsonType: "string" },
        media: {
          bsonType: "array",
          items: { bsonType: "string" }
        },
        startDate:   { bsonType: "date" },
        endDate:     { bsonType: "date" },
        status:      { enum: ["active","closed","scheduled"] },
        bidsPreview: {
          bsonType: "array",
          items: {
            bsonType: "object",
            required: ["userId","amount","placedAt"],
            properties: {
              userId:   { bsonType: "objectId" },
              amount:   { bsonType: "double" },
              placedAt: { bsonType: "date" }
            }
          }
        }
      }
    }
  }
});

// 4. Kolekcja auction.bids (pełna historia)
db.createCollection("auction.bids", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["auctionId","userId","amount","placedAt"],
      properties: {
        auctionId: { bsonType: "objectId" },
        userId:    { bsonType: "objectId" },
        amount:    { bsonType: "double", minimum: 0 },
        placedAt:  { bsonType: "date" }
      }
    }
  }
});

// 5. Kolekcja auction.history
db.createCollection("auction.history", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["auctionId","winnerUserId","bids","closedAt"],
      properties: {
        auctionId:    { bsonType: "objectId" },
        winnerUserId: { bsonType: "objectId" },
        finalPrice:   { bsonType: "double" },
        bids: {
          bsonType: "array",
          items: { bsonType: "objectId" }
        },
        closedAt:     { bsonType: "date" }
      }
    }
  }
});

// 6. Kolekcja log
db.createCollection("log", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["entity","action","timestamp"],
      properties: {
        entity:    { enum: ["user","auction","bid"] },
        action:    { bsonType: "string" },
        timestamp: { bsonType: "date" },
        meta:      { bsonType: "object" }
      }
    }
  }
});

```
---

### Ładowanie danych z dumpa

Aby załadować dane z dumpa bazy `AuctionDatabase`, użyj jednej z poniższych komend:

1. **Podstawowe ładowanie:**
   ```bash
   mongorestore ./AuctionDatabase
   ```
   - Ładuje dane z lokalnego dumpa do domyślnej instancji MongoDB.

2. **Ładowanie z pełnym connection stringiem:**
   ```bash
   mongorestore --uri="mongodb://localhost:27017" --db=AuctionDatabase ./AuctionDatabase
   ```
   - Określa połączenie i bazę danych, do której mają trafić dane.

3. **Nadpisanie istniejącej bazy danych:**
   ```bash
   mongorestore --uri="mongodb://localhost:27017" --drop --db=AuctionDatabase ./AuctionDatabase
   ```
   - Usuwa istniejące dane przed załadowaniem nowych.

---

## Wariant 1: Znormalizowany (referencje)

### Struktura:
- **users** – Przechowuje informacje o użytkownikach.
  - nazwa
  - e-mail
  - hasło (hash)
  - lista refresh tokenów

- **auction** – Przechowuje dane o aktywnych aukcjach.
  - id aukcji
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu (lista URL)
  - data początku
  - data zakończenia
  - status akcji (np. active / closed)

- **auction.bids** – Przechowuje wszystkie oferty (bids) na aukcje jako osobna kolekcja.
  - id aukcji (referencja do `auction`)
  - user id (referencja do `users`)
  - wystawiona cena
  - data wystawienia (milisekundy / ISODate)

- **auction.history** – Przechowuje informacje o zakończonych aukcjach.
  - id aukcji (referencja)
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu
  - data początku
  - data zakończenia
  - wygrana przez user id (referencja)
  - lista bidów (referencje do `auction.bids`)
  - status akcji

- **log** – Przechowuje informacje o operacjach wykonanych w ramach bazy.
  - typ encji (np. user / auction / bid)
  - id encji
  - akcja (np. create / update / delete)
  - data wykonania
  - dodatkowe dane operacji (meta)

### Zalety:
- Lepsza skalowalność – dane można łatwo rozproszyć między kolekcje.
- Redukcja duplikacji danych (np. opis aukcji nie jest powielany w historii).
- Ułatwia kontrolę spójności przy użyciu walidacji i relacji.

### Wady:
- Więcej zapytań i joinów (przez `$lookup`) – większe obciążenie przy agregacjach.
- Trudniejsze do wdrożenia w systemach z wysoką liczbą zapytań.
- Czasochłonniejsze dla prostych operacji.

### Przykładowe operacje:
TODO
---

## Wariant 2: Zagnieżdżone dokumenty (denormalizacja)

- **users**
  - nazwa
  - e-mail
  - hasło (hash)
  - lista refresh tokenów

- **auction** – Przechowuje dane o aukcji **wraz z ofertami**.
  - id aukcji
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu
  - data początku
  - data zakończenia
  - status akcji
  - bids (tablica zagnieżdżonych dokumentów)
    - user id
    - wystawiona cena
    - data wystawienia

- **auction.history** – Zakończone aukcje wraz z pełną historią bidów.
  - id aukcji
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu
  - data początku
  - data zakończenia
  - wygrana przez user id
  - lista bidów (wbudowana tablica obiektów)
    - user id
    - wystawiona cena
    - data wystawienia
  - status akcji

- **log**
  - typ operacji
  - czas
  - meta
  - nazwa kolekcji / dokumentu


### Zalety:
- Bardzo szybki dostęp do pełnych danych aukcji w jednym zapytaniu.
- Mniejsza liczba joinów i operacji agregujących oznacza lepszą wydajność w prostych przypadkach.
- Dobra współpraca tego rozwiązania z MongoDB (pojedyńczy plik JSON zamiast kilku kolekcji).

### Wady:
- Duplikacja danych może powodować trudniejsze aktualizacje danych.
- Dokumenty mogą szybko osiągnąć limit 16MB.

### Przykładowe operacje:
TODO

---

## Wariant 3: połączenie powyższych (embedowanie + referencje)

- **users**
  - nazwa
  - e-mail
  - hasło (hash)
  - lista refresh tokenów

- **auction**
  - id aukcji
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu
  - data początku
  - data zakończenia
  - status akcji
  - bidsPreview (ostatnie 5–10 ofert)
    - user id
    - wystawiona cena
    - data wystawienia

- **auction.bids** – Wszystkie oferty jako osobna kolekcja (pełna historia).
  - id aukcji
  - user id
  - wystawiona cena
  - data wystawienia

- **auction.history**
  - id aukcji
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu
  - data początku
  - data zakończenia
  - wygrana przez user id
  - lista bidów (referencje do dokumentów w `auction.bids`)
  - status akcji

- **log**
  - typ operacji
  - czas
  - obiekt (np. aukcja, bid)
  - identyfikator obiektu
  - dane dodatkowe


### Zalety:
- Łączy zalety obu podejść – szybki dostęp do najważniejszych danych + pełna historia w osobnej kolekcji.
- Optymalizacja pod kątem wydajności (przeglądanie aukcji) i elastyczności (analiza historii).
- Umożliwia budowanie funkcji typu "ostatnie oferty" bez konieczności pełnego przeszukiwania bazy.

### Wady:
- Większa złożoność implementacyjna (dane częściowo zagnieżdżone, częściowo referencyjne).
- Wymaga dbałości o spójność między kolekcjami (np. bidsPreview vs auction.bids).
- Trudniejsze w debugowaniu i utrzymaniu.

### Przykładowe operacje:
TODO

---

Punktacja:

|         |     |
| ------- | --- |
| zadanie | pkt |
| 1       | 1   |
| 2       | 1   |
| razem   | 2   |



