<!--

author:   Volker Göhler
email:    volker.goehler@informatik.tu-freiberg.de
version:  0.0.3
language: de
narrator: Deutsch Female

edit: true
date: 2026-06-04

comment:  Übung Softwareentwicklung 05 -- User Story und UML

import: https://raw.githubusercontent.com/liascript-templates/plantUML/master/README.md

link:   https://raw.githubusercontent.com/vgoehler/LiaScript_CSS_Provider/refs/heads/main/dist/university.css

tags: [Sommersemester2026, Softwareentwicklung, Übung05]

-->

[![LiaScript Course](https://raw.githubusercontent.com/LiaScript/LiaScript/master/badges/course.svg)](https://liascript.github.io/course/?https://raw.githubusercontent.com/Ifi-Softwareentwicklung-SoSe2026/exercise-05-FelixSiegel/refs/heads/main/README.md)

# Aufgabe 05

Softwareentwicklung SoSe2026
============================

## User Story

Als Benutzer eines Fußball-Wettannahmesystems
möchte ich Wetten auf die Spiele der Fußball-Weltmeisterschaft (inkl. Gruppenphase) abgeben und verwalten,
damit ich meine Prognosen für die Spiele dokumentieren und Wetten mit Quoten versehen kann.

### Akzeptanzkriterien

Turnierstruktur
--------------------

- Das System modelliert die Gruppenphase der Fußball-Weltmeisterschaft.
- Jede Gruppe enthält Mannschaften (Klasse Mannschaft mit Name).
- Jedes Spiel (Klasse Spiel) hat:

   - Zwei Mannschaften (Heim und Auswärts).
   - Ein Datum und eine Uhrzeit.
   - Ein Ergebnis (initial leer).
   - Eine eindeutige Spiel-ID.


Wettfunktionalität
--------------------

- Jedes Spiel kann Wettquoten für verschiedene Wett-Typen speichern (z. B. Siegwette, Ergebniswette).
- Eine Wette (Klasse Wette) besteht aus:

   - Einem Benutzer (Klasse Benutzer mit Name und Guthaben).
   - Einem Wett-Typ (z. B. Siegwette, Ergebniswette).
   - Einer Quote (z. B. 1.80, 3.50).
   - Einem Einsatz (Geldbetrag).

Datenpersistenz
--------------------

- Die Turniertabelle (Spiele und Mannschaften) muss in einer Datei gespeichert und geladen werden können (z. B. JSON).
- Wetten und Quoten werden ebenfalls persistent gespeichert.

Benutzerinteraktion (Konsolenanwendung)
--------------------

Das System wird über Command-Line-Parameter gesteuert:

- `new`: Initialisiert die Turniertabelle (statisch, da das Turnier vorgegeben ist).
- `print`: Gibt alle Spiele der Tabelle mit ihren IDs aus.
- `set <spielid> <Wetttyp> <Wettquote>`: Setzt eine Wettquote für ein Spiel und einen Wett-Typ.
- `get <spielid> <Wetttyp>`: Gibt die aktuelle Wettquote für ein Spiel und einen Wett-Typ aus.
- `bid <player> <spielid> <Wetttyp> <amount>`: Platziert eine Wette für einen Benutzer.
- `result <spielid> <Tore-1.Mannschaft>:<Tore-2.Mannschaft>`: Trägt das Spielergebnis ein und löst die Auswertung der Wetten aus.


### Stufenweise Implementierung

Design und Implementierung soll jeweils in einem Branch stattfinden.
Jede Stufe soll funktional sein und die vorherige Stufe erweitern.
Die Stufen sollen einzeln in main gemerged werden, damit die Funktionalität schrittweise erweitert wird.

### Start eines C#-Projekts

Legen Sie für die Implementierung ein eigenes .NET-Konsolenprojekt an. Ein möglicher Startpunkt ist:

```bash
mkdir betting_system
dotnet new console --framework net10.0 --output betting_system
dotnet restore betting_system/betting_system.csproj
dotnet build betting_system/betting_system.csproj
dotnet run --project betting_system/betting_system.csproj -- new
```

Arbeiten Sie danach nicht direkt auf `main`, sondern erstellen Sie für jeden Aufgabenteil einen eigenen Branch und öffnen Sie eine Pull Request nach `main`.

**Wichtig für die automatische Auswertung (CI):**
Erweitern Sie Ihre `Program.cs` so, dass bei einem Programmstart ohne Argumente (`dotnet run`) ein **automatisierter Testablauf** ausgeführt wird. Rufen Sie in diesem Fall programmgesteuert nacheinander die Befehle auf, die Sie bisher implementiert haben (z. B. Turnier initialisieren, eine Test-Quote setzen, eine Test-Wette platzieren, ein Ergebnis eintragen und am Ende die Tabellen und Wetten ausgeben). Der automatische `dotnet-build` Workflow führt `dotnet run` ohne Argumente aus und fängt diese Ausgabe ab, damit Lisa (AI-Agent) anhand der Konsolenausgabe prüfen kann, ob Ihre Logik fehlerfrei funktioniert.

## Agent-Workflow

Erstellen Sie zu Beginn ein GitHub-Issue, in dem Sie Kevin bitten, aus dieser User Story ein PlantUML-Klassendiagramm zu erstellen.

Kevin arbeitet auf einem eigenen Branch, aktualisiert die `README.md` und erstellt eine Pull Request nach `main`. Kevin soll dabei auch den LiaScript-Course-Link auf Ihr studentisches Repository anpassen.

Reviewen Sie Kevins Pull Request. Lisa reviewed denselben Pull Request automatisch. Kevin arbeitet Feedback erst ein, wenn für denselben PR-Stand sowohl Ihr Review als auch Lisas Review vorliegen. Der Pull Request wird erst gemerged, wenn beide Reviews genehmigt sind.

Stufe 1 (Grundlage):
--------------------

- Modellierung der Turniertabelle (Mannschaften und Spiele).
- Speichern/Laden der Turnierdaten.
- `new` und `print` Befehle implementieren.

Stufe 2 (Erweitert):
--------------------

- Hinzufügen von Wettquoten für Spiele.
- `set` und `get` Befehle implementieren.

Stufe 3 (Fortgeschritten):
--------------------

- Platzieren von Wetten durch Benutzer (`bid`-Befehl).
- Verwaltung von Benutzer-Guthaben.
- Eingabe von Spielergebnissen und Auswertung der Wetten (`result`-Befehl).


## Aufgabe: PlantUML-Antwortfeld

Kevin ersetzt den folgenden Platzhalter mit einem LiaScript-kompatiblen PlantUML-Block. Der erzeugte Block enthält `@plantUML.eval(png)`, damit LiaScript das Diagramm rendert.

- [plantUml Editor](https://pantuml.com)
- paste and copy your code! Mit Reloads verlieren Sie Ihre Eingaben, daher vorher sichern!

<!-- kevin-uml:start -->
```text @plantUML
@startuml
skinparam classAttributeIconSize 0

class TurnierManager {
    - games: List<Spiel>
    - teams: List<Mannschaft>
    - users: List<Benutzer>
    + initializeTurnier(): void
    + saveToFile(filename: string): void
    + loadFromFile(filename: string): void
    + getSpielById(id: int): Spiel
    + getBenutzerByName(name: string): Benutzer
}

class Mannschaft {
    + name: string
}

class Spiel {
    + spielId: int
    + homeTeam: Mannschaft
    + awayTeam: Mannschaft
    + datum: DateTime
    + uhrzeit: Time
    + ergebnis: string
    - quotes: List<Wettquote>
    + setQuote(type: string, value: double): void
    + getQuote(type: string): double
    + setResult(score: string): void
}

class Wettquote {
    + wetttyp: string
    + quote: double
}

class Benutzer {
    + name: string
    + guthaben: double
    + updateBalance(amount: double): void
}

class Wette {
    + wetttyp: string
    + quote: double
    + einsatz: double
    + isEvaluated: boolean
    + evaluate(actualResult: string): double
}

TurnierManager "1" o-- "*" Spiel
TurnierManager "1" o-- "*" Mannschaft
TurnierManager "1" o-- "*" Benutzer

Spiel "*" o-- "2" Mannschaft : teilnehmend
Spiel "1" *-- "*" Wettquote : besitzt

Wette "*" --> "1" Benutzer : platziert von
Wette "*" --> "1" Spiel : bezieht sich auf

note right of TurnierManager : Steuert Persistenz
und CLI-Logik
@enduml
```
@plantUML.eval(png)
<!-- kevin-uml:end -->
