# Bazy Danych
## Entity Framework
#### Autorzy

**`Grupa 8 - [Śr 12:15 , A]`**

- Bartosz Ludwin
- Mateusz Pawliczek
- Filip Malejki

Na zajęciach udało się zrealizować `Część I` oraz podpunkty `a` i `b` z `Części II`. Reszta zadań została zrealizowana po zajęciach.

## Część I:

![alt text](./task%20one/dotnet-task-one-prodContext.png)
![alt text](./task%20one/dotnet-task-one-structure.png)
![alt text](./task%20one/dotnet-task-one.png)
___
## Część II:

## A) Tworzenie tabel
![alt text](./IIa/tables.png)
Dodanie relacji:
![alt text](./IIa/dbcontext.png)
Diagram bazy (wygenerowany w DataGrip):
![alt text](./IIa/scheme.png)
Dodanie nowego suppliera (podpunkt i)
![alt text](./IIa/new-supplier.png)
Dodanie stworzonego dostawcy do wcześniejszego produktu (podpunkt ii)
![alt text](./IIa/code.png)
Wynik przed:
![alt text](./IIa/before.png)
Wynik po:
![alt text](./IIa/after.png)

___

## B) Zmieniamy relację
![alt text](./IIb/relation.png)
Dodajemy kilka produktów do nowego dostawcy
![alt text](./IIb/code.png)
Wynik przed:
![alt text](./IIb/after.png)
Wynik po:
![alt text](./IIb/before.png)

___

# C) Modyfikacja relacji
![alt text](./IIc/relation.png)
Dodajemy kilka produktów do nowego dostawcy
![alt text](./IIc/code.png)
Wynik po:
![alt text](./IIc/after.png)

___

# D) Stworzenie nowych tabel
### (dodajemy również pośrednią encję InvoiceProduct, ponieważ inaczej nie moglibyśmy dodać wartości "Quantity")
![alt text](./IId/tables.png)
Dodanie relacji:
![alt text](./IId/relations.png)
Schemat graficzny:
![alt text](./IId/scheme.png)
Dodanie kilku produktów i sprzedanie ich w ramach faktur:
![alt text](./IId/code.png)
Kod wyświetlający produkty sprzedane w ramach wybranej faktury/transakcji:
![alt text](./IId/ii.png)
Kod wyświetlający faktury, w ramach których sprzedany został wybrany produkt:
![alt text](./IId/iii.png)
Wynik:
![alt text](./IId/result.png)
___

# E) Dodanie nowych tabel:
![alt text](./IIe/tables.png)
Dodanie relacji między nimi
![alt text](./IIe/relations.png)
Schemat:
![alt text](./IIe/scheme.png)
Kod dodający klienów:
![alt text](./IIe/adding-clients.png)
Kod wyświetlających klientów:
![alt text](./IIe/printing-clients.png)
Wynik:
![alt text](./IIe/results.png)

___

# F) Strategia `Table-Per-Type`
Użyte tabele (bez zmian)
![alt text](./IIf/tables.png)
Nowa modyfikacja relacji:
![alt text](./IIf/relations.png)
Schemat:
![alt text](./IIf/scheme.png)
Kod dodający klienów:
![alt text](./IIf/adding-clients.png)
Kod wyświetlających klientów:
![alt text](./IIf/printing-code.png)
Wynik:
![alt text](./IIf/results.png)

# G) Porównanie obu strategii
Strategie **TPH** `(Table-per-Hierarchy)` i **TPT** `(Table-per-Type)` różnią się sposobem odwzorowania dziedziczenia w relacyjnej bazie danych, a co za tym idzie: wydajnością, przejrzystością danych i ich spójnością.

**TPH** umieszcza całą hierarchię dziedziczenia w jednej tabeli. Skutkuje to tym, że wszystkie właściwości z klas bazowych i pochodnych znajdują się w jednej strukturze danych, a nieużywane kolumny dla danego rekordu pozostają puste. TPH upraszcza model bazy, jest bardziej wydajna przy odczytach, ponieważ eliminuje konieczność używania złożonych zapytań z JOINami. Jednak ta prostota i wydajność okupiona jest mniejszą przejrzystością danych, trudnością w zapewnieniu spójności oraz obecnością wielu NULL-i w kolumnach nieistotnych dla konkretnego typu.

Z kolei **TPT** rozdziela strukturę danych między osobne tabele – każda klasa dziedzicząca ma własną tabelę zawierającą wyłącznie swoje właściwości, a dane wspólne są przechowywane w tabeli bazowej. Zapytania do takich danych wymagają dołączeń między tabelami, co znacząco wpływa na wydajność przy dużych zbiorach danych lub częstym odczycie. TPT zapewnia jednak większą przejrzystość modelu i pozwala zachować ścisłą spójność danych (np. pola typowe tylko dla PublicCompany nie mogą pojawić się przypadkowo w rekordzie PrivateCompany). Struktura jest bliższa obiektowemu dziedziczeniu i czytelniejsza, szczególnie przy bardziej rozbudowanych hierarchiach klas.

