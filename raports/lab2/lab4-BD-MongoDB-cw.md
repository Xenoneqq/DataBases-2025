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
  - JWT refresh token

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

Punktacja:

|         |     |
| ------- | --- |
| zadanie | pkt |
| 1       | 1   |
| 2       | 1   |
| razem   | 2   |



