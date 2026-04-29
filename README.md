# HP Tourist

## Project omschrijving

Huisartsenpraktijk Tourist Doctor Amsterdam krijgt een stroom Poolse toeristen de komende maanden.\
De toeristen krijgen allemaal diverse medicatie van hun Poolse huisarts.\
De toeristen spreken geen Engels en geen Nederlands.\
Leeftijd varieert van 3 tot 70 jaar oud.\
Eén patiënt heeft leukemie.\
De toeristen hebben niet voldoende medicatie mee vanuit huis en zullen gedurende hun vakantie
herhaalrecepten aan moeten vragen bij de huisartsenpraktijk in Amsterdam.\
Niet alle medicatie uit Polen komt 1-op-1 overeen met de Nederlandse medicatie.\
Ook wordt er in Nederland strenger gecontroleerd op eventuele contra-indicaties, interacties en allergieën.
 
Vraag:\
De huisartsenpraktijk heeft bij jullie een verzoek gedaan om een app te ontwikkelen waarbij deze toeristen zich kunnen inschrijven bij de praktijk, een herhaalrecept aanvraag kunnen doen en waarbij een medicatielijst ingezien kan worden.\
De huisarts moet de aangevraagde medicatie kunnen voorschrijven volgens de Nederlandse standaarden en declareren bij de reisverzekering van de toerist.
 
Tips:
- Houd rekening met taal.
- Houd rekening met visualisatie.
- Houd rekening met contra-indicaties / interacties / allergieën.
- Houd rekening met uitwisseling van gegevens van en naar de Poolse zorginstanties.
- Gebruik https://www.farmacotherapeutischkompas.nl voor informatie over geneesmiddelen. Mogelijk is er zelfs een api beschikbaar om de informatie op te halen in jullie applicatie.
 
Aanpak:
- Maak een functioneel ontwerp.
- Maak een MoSCoW analyse.
- Maak een sprintplanning met taken.
- Maak een haalbaarheidsinschatting.
- Maak de app voor de toeristen en de scherm(en) voor de arts
- Gebruik Blazor!
- Zet het werk in GitHub zodat jullie dit met elkaar kunnen delen.
- Plan onderlinge reviews van de code.
- Schakel voor functionele en proces vragen met de stakeholder, oftewel de huisarts.
- Geef een (eind)presentatie aan de huisarts.

## Entity Framework

### Voorbereiding

Installeer de Entity Framework CLI tool als je die nog niet hebt met:

    dotnet tool install --global dotnet-ef


### Migraties maken

Na het wijzigen van de database kun je een migratie maken met:

    dotnet ef migrations add {MigratieNaam}

### Migraties uitvoeren

De migraties uitvoeren kan met:

    dotnet ef database update

## Authenticatie

### Registreren (`/register`)

Een toerist maakt een patiëntaccount aan met voornaam, achternaam, geboortedatum, geslacht, EHIC-nummer + vervaldatum, e-mail en wachtwoord. Validaties draaien client- en serverside via DataAnnotations:

- Voor- en achternaam: verplicht, max. 100 tekens.
- Geboortedatum: verplicht, leeftijd tussen 0 en 100 jaar (`AgeRangeAttribute`).
- Geslacht: verplicht (keuze uit de `Gender` enum).
- EHIC-nummer: verplicht, exact 20 alfanumerieke tekens, **uniek over alle accounts** (gecontroleerd in de service plus afgedwongen door een unique index op `EHICs.EncryptedEHICNumber`); vervaldatum moet in de toekomst liggen (`FutureDateAttribute`).
- E-mail: verplicht, geldig e-mailadres, uniek over alle accounts.
- Wachtwoord: min. 8 tekens; bevestiging moet matchen (`Compare`).

De praktijk wordt automatisch op Huisartsenpraktijk Tourist Doctor Amsterdam gezet (geseed via `SeededIds`). Bij succes wordt direct ingelogd.

### Inloggen (`/login`)

Authenticatie loopt via cookies (geen JWT). De cookie heet `HPTourist.Auth`, heeft een sliding expiry van 8 uur en wordt door `PatientAccountService` geschreven met `HttpContext.SignInAsync`. Wachtwoorden worden geverifieerd met `IPasswordHasher<User>` (PBKDF2). De claims bevatten `NameIdentifier`, `Name` (e-mail), `Role`, en — afhankelijk van het accounttype — `PatientId`/`EmployeeId` plus voor- en achternaam.

> **Let op:** de auth-pagina's draaien in static SSR-mode, niet in Interactive Server. De cookie moet via response headers op de POST geschreven worden, en dat kan niet via de Blazor-circuit.

### Uitloggen (`/logout`)

Verwijdert de cookie via `SignOutAsync` en stuurt de gebruiker terug naar de homepagina met `?msg=loggedOut`.

## Localisatie 

### Resources
In de resource files staan alle keys en bijbehorende tekst in engels, nederlands of pools. De keys kunnen worden gebruikt in de code in plaats van de tekst. De resource files volgen dezelfde structuur als de rest van het project, de homepagina staat bijvoorbeeld HPTourist/Components/Pages/Home.razor en de resources(met de tekst die nodig is op die pagina) staan vervolgens in Resources/Components/Pages/Home.en.resx etc. Elke nieuwe component heeft dus zijn eigen groepje files nodig.

### Gebruik in components
Gebruik @inject IStringLocalizer<"*naamComponent*"> Localizer . Vervolgens gebruik je op de plek waar je vertaalbare tekst wilt hebben de localizer met de naam van de key ipv. de tekst <h1>Welcome</h1> wordt bijvoorbeeld <h1>@Localizer["Welcome"]</h1>.
Andere voorbeelden:
 placeholder="@((string)Localizer["EHICPlaceholder"])"
<p>@string.Format(Localizer["Welcome"], userName)</p> value:Welcome  key:"Hello, {0}!"
 @foreach (var g in Enum.GetValues<Gender>())
                {
                    <option value="@g">@Localizer[g.ToString()]</option>
                }