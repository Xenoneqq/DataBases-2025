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

# f)

Napisz polecenie które modyfikuje zamówienie dodane w pkt e)  zwiększając zniżkę  o 5% (dla każdej pozycji tego zamówienia) 

Napisz polecenie 
- aktualizując oryginalną kolekcję `orderdetails`
- aktualizując kolekcję `OrderInfo`
- aktualizując kolekcję `CustomerInfo`



UWAGA:
W raporcie należy zamieścić kod poleceń oraz uzyskany rezultat, np wynik  polecenia `db.kolekcka.fimd().limit(2)` lub jego fragment


## Zadanie 1  - rozwiązanie

> Wyniki: 
> 
> przykłady, kod, zrzuty ekranów, komentarz ...

a)

```js
--  ...
```

b)


```js
--  ...
```

....

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

### a) Struktura bazy danych

Baza danych będzie posiadać następujące tabele:

- **users** - Przechowuje informacje o użytkownikach:
  - nazwa
  - mail
  - hasło (hash)
  - JWT refresh token (pominięty w implementacji ze względu na brak backendu)

- **auction** - Posiada informacje o obecnie trwających aukcjach. Przechowane informacje to: 
  - id aukcji
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu
  - data początku
  - data zakończenia

- **auction.bids** - Przechowuje informacje o osobach które obstawiają swoje pieniądze w celu wygrania aukcji.
  - user id
  - wystawiona cena
  - data wystawienia (milisec)

- **auction-history** - Przechowuje informacje o zakończonych aukcjach.
  - id aukcji
  - nazwa
  - kategoria
  - opis przedmiotu
  - media przedmiotu
  - data początku
  - data zakończenia
  - wygrana przez user id
  - lista bidów

- **log** - przechowuje informacje o operacjach wykonanych w ramach bazy.


### komentarz do struktury danych

- Przechowywanie ofert `bids` w kolekcji `action` sprawia, że wszystkie powiązane z aukcją transakcje (osoby do niej przypisane), są natychmiastowo związane z daną aukcją. Dodatkowo, w przypadku usunięcia aukcji, usuwane są również osoby przypisane do tej aukcji.

- Tabele taka jak `log` mogłaby działąć tylko z użyciem procedur lub triggerów, których czysty mongodb nie obsługuje. Implementacja takich mechanizmów musiałaby zostać zrealizowana w ramach backendu pisanego w node.js / flusk / dowolnym innym backendowym środowisku. W związku z tym tabela `log` zostanie pominięta w poniższej implementacji.

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

### Przykładowe dane do INSERT

#### wstawianie do User

Informacje do wstawiania (backend powinien je weryfikować)
- UserID nie powinno się powtarzać
- Email powinien być unikatowy
- Hasło jest hashowane

```mongodb
db.users.insertOne({
    UserID:0,
    Email:"nicemail@gmail.com",
    Password:"uu3451sfgias",
})
```

#### wstawianie do Auction

- AuctionID nie może się powtarzać
- Item media zawiera listę zdjęć przedmiotu
- Bids powinny być ustawione rosnąco względem obstawianej ceny (inaczej mówiąc największy Bid jest na końcu listy)

```mongodb
db.auction.insertOne({
    AuctionID:0,
    Name:"Auction Name",
    Category:"Item Category",
    ItemDescription:"Item Description",
    ItemMedia:[
      "Link to title image",
      "Link to media one",
      "Link to media two",
      "Link to media three",
      ...
    ],
    StartDate:"2025-05-03T19:54:00.123Z",
    EndDate:"2026-01-01T19:54:00.123Z",
    Bids:[
        {UserID:0, BidUSD:10, BidTime:"2025-05-03T19:54:00.123Z"},
        {UserID:1, BidUSD:20, BidTime:"2025-05-03T19:54:00.223Z"},
        {UserID:2, BidUSD:30, BidTime:"2025-05-03T19:54:00.333Z"},
        ...
    ]
})
```

#### wstawianie do Auction-History

- AuctionID nie może się powtarzać
- Item media zawiera listę zdjęć przedmiotu
- WinnerID zawiera id zwycięzcy aukcji (jest to ID usera na ostatnim miejscu w tabeli bids)

```mongodb
db.auction-history.insertOne({
    AuctionID:0,
    Name:"Auction Name",
    Category:"Item Category",
    ItemDescription:"Item Description",
    ItemMedia:[
      "Link to title image",
      "Link to media one",
      "Link to media two",
      "Link to media three",
      ...
    ],
    StartDate:"2025-05-03T19:54:00.123Z",
    EndDate:"2026-01-01T19:54:00.123Z",
    WinnerID:2,
    Bids:[
        {UserID:0, BidUSD:10, BidTime:"2025-05-03T19:54:00.123Z"},
        {UserID:1, BidUSD:20, BidTime:"2025-05-03T19:54:00.223Z"},
        {UserID:2, BidUSD:30, BidTime:"2025-05-03T19:54:00.333Z"},
        ...
    ]
})
```

### Możliwe zapytania i operacje na bazie danych

Tutaj będą zapytania / operacje


Punktacja:

|         |     |
| ------- | --- |
| zadanie | pkt |
| 1       | 1   |
| 2       | 1   |
| razem   | 2   |



