# Testovací scénáře pro Soul Knight MUD

**Tested Project Name:** Soul Knight MUD

Tento dokument obsahuje sadu testovacích scénářů (Test Cases) určených pro manuální testování aplikace Soul Knight MUD. Testy jsou napsány tak, aby je mohl provést kdokoli bez předchozí znalosti kódu a naformátovány podle šablony SPŠE Ječná.

## Příprava a testovací účty

Před začátkem testování se ujistěte, že je server spuštěn (port 4000) a máte k dispozici klientskou aplikaci. K testování budete potřebovat dva spuštěné klienty.

| Typ hráče | Uživatelské jméno | Heslo | Stav před testem |
| :--- | :--- | :--- | :--- |
| **Hráč 1 (Hlavní testovací)** | `tester_01` | `Heslo123` | Účet je potřeba nejprve vytvořit (viz test MVP05) |
| **Hráč 2 (Sekundární)** | `tester_02` | `Heslo123` | Účet je potřeba vytvořit a nechat přihlášený pro testy interakce |
| **Neexistující hráč** | `ghost_player_999` | `Cokoliv` | Účet nesmí existovat |
| **Hráč pro dokončení hry** | `hero_99` | `Hero123` | Účet pro testování výhry (P1) |

---

## 1. MVP Funkce (Minimum Viable Product)

### SPSE Jecna Test Case: MVP01
| Test Case ID: MVP01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Spuštění serveru | |
| **Brief description:** Ověření, že herní server lze úspěšně spustit a naslouchá. | |
| **Pre-conditions:** Aplikace je zkompilována. Port 4000 je volný. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Otevřete terminál ve složce `Server` a spusťte příkaz pro start. | `dotnet run` | V konzoli serveru se vypíše zpráva `[SERVER] Herní svět načten...` a `[SERVER] Nasloucháme na portu 4000 (max 20 hráčů)...`. Server nespadne a běží. | Lze použít klávesu F5 ve VS Code. |

<br><br>

### SPSE Jecna Test Case: MVP02
| Test Case ID: MVP02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Připojení klienta | |
| **Brief description:** Ověření úspěšného připojení do MUDu z klientské aplikace. | |
| **Pre-conditions:** Server běží podle MVP01. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Otevřete nový terminál ve složce `Client` a zapněte program. | IP: `localhost`, Port: `4000` (výchozí) nebo `dotnet run` | Klient se připojí, zobrazí uvítací banner hry s textem "Soul Knight MUD" a vypíše nabídku k přihlášení (`[1] Přihlásit se`, `[2] Vytvořit nový účet`). | Lze použít i PuTTY. |

<br><br>

### SPSE Jecna Test Case: MVP03
| Test Case ID: MVP03 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Neúspěšné přihlášení (neexistující účet) | |
| **Brief description:** Ověření odmítnutí přihlášení v případě, že jméno hráče není v databázi. | |
| **Pre-conditions:** Klient je připojen k serveru. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zvolte možnost přihlášení. | `1` | Server vyzve k zadání jména. | |
| 2 | Zadejte jméno neexistujícího hráče a po výzvě i libovolné heslo. | Jméno: `ghost_player_999`, Heslo: `Cokoliv` | Server zobrazí zprávu `Špatné jméno nebo heslo.` a znovu zobrazí úvodní nabídku pro přihlášení/registraci. | |

<br><br>

### SPSE Jecna Test Case: MVP04
| Test Case ID: MVP04 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Neúspěšné přihlášení (špatné heslo) | |
| **Brief description:** Ověření ochrany účtu pomocí hesla. | |
| **Pre-conditions:** Existuje hráč `tester_02`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zvolte možnost přihlášení. | `1` | Server vyzve k zadání jména. | |
| 2 | Zadejte jméno existujícího hráče a k němu nesprávné heslo. | Jméno: `tester_02`, Heslo: `SpatneHeslo999` | Server zobrazí zprávu `Špatné jméno nebo heslo.` a znovu zobrazí úvodní nabídku. | |

<br><br>

### SPSE Jecna Test Case: MVP05
| Test Case ID: MVP05 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Registrace nového hráče | |
| **Brief description:** Zajištění správného vytvoření účtu. | |
| **Pre-conditions:** Klient je připojen, účet `tester_01` ještě neexistuje. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | V nabídce zvolte volbu pro nový účet a zadejte nová data. | Volba: `2`, Jméno: `tester_01`, Heslo: `Heslo123` | Zobrazí se zprávu `Účet 'tester_01' vytvořen! Dobrodružství začíná...`. Následně se automaticky načte první místnost ("Vstupní hala hradu") a zobrazí se herní prompt `[tester_01 | HP:100]>`. | |

<br><br>

### SPSE Jecna Test Case: MVP06
| Test Case ID: MVP06 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Úspěšné přihlášení existujícího hráče | |
| **Brief description:** Ověření možnosti standardního loginu k účtu. | |
| **Pre-conditions:** Účet `tester_01` existuje (vytvořen v MVP05). Hráč aktuálně není připojen. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Spusťte klienta, zvolte přihlášení a zadejte validní data. | Volba: `1`, Jméno: `tester_01`, Heslo: `Heslo123` | Zobrazí se uvítací zpráva `Vítej zpět, tester_01!` a vypíše se obsah aktuální místnosti. Hráč se ocitne v herní smyčce. | |

<br><br>

### SPSE Jecna Test Case: MVP07
| Test Case ID: MVP07 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Low | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Příkaz pomoc | |
| **Brief description:** Kontrola dostupnosti a čitelnosti in-game nápovědy. | |
| **Pre-conditions:** Hráč je přihlášen ve hře. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Do herního promptu zadejte příkaz pro nápovědu. | `pomoc` | Zobrazí se strukturovaná tabulka s nápovědou rozdělená do kategorií (POHYB, PŘEDMĚTY, atd.) obsahující syntaxi příkazů. | |

<br><br>

### SPSE Jecna Test Case: MVP08
| Test Case ID: MVP08 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Zobrazení místnosti (prozkoumej) | |
| **Brief description:** Ověření rozhlížení po lokaci. | |
| **Pre-conditions:** Hráč je přihlášen (např. v místnosti Vstupní hala hradu). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zadejte příkaz k prozkoumání místnosti. | `prozkoumej` (nebo `look`) | Vypíše se ohraničený blok s názvem místnosti, jejím popisem, dostupnými východy (např. `sever`), seznamem předmětů a přítomnými NPC. | |

<br><br>

### SPSE Jecna Test Case: MVP09
| Test Case ID: MVP09 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Pohyb mezi místnostmi | |
| **Brief description:** Ověření základního MUD pohybu hráče na světové mapě. | |
| **Pre-conditions:** Hráč je ve Vstupní hale, kde je východ na sever. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkuste jít neplatným směrem. | `jdi zapad` | Server odpoví `Směrem 'zapad' se jít nedá.`. Hráč setrvá v hale. | |
| 2 | Zkuste jít platným směrem. | `jdi sever` | Hráč se přesune do místnosti "Zbrojnice" a automaticky se vypíše její popis a obsah. | |

<br><br>

### SPSE Jecna Test Case: MVP10
| Test Case ID: MVP10 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Sebrání a odložení předmětu | |
| **Brief description:** Ověření manipulace s itemy na podlaze. | |
| **Pre-conditions:** Hráč je v místnosti s předmětem (ve Vstupní hale je `pochodna`). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkuste sebrat neexistující předmět. | `vezmi blbost` | `Předmět 'blbost' tu nikde nevidíš.` | |
| 2 | Zkuste sebrat existující předmět. | `vezmi pochodna` | `Vezmeš: pochodna. [Popis předmětu]` a předmět zmizí z místnosti. | |
| 3 | Předmět následně odložte. | `odlož pochodna` | `Odložíš pochodna na zem.` a předmět se znovu objeví v místnosti (viditelné po příkazu `prozkoumej`). | |

<br><br>

### SPSE Jecna Test Case: MVP11
| Test Case ID: MVP11 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Inventář a překročení kapacity | |
| **Brief description:** Ověření funkcionality váhového limitu a výpisu. | |
| **Pre-conditions:** Hráč má ve Vstupní hale předměty, jejichž celková váha přesahuje kapacitu (15). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Seberte těžké předměty, dokud nenarazíte na limit. | `vezmi mec`, `vezmi stit`... | Při překročení kapacity server vypíše např. `Nemůžeš vzít 'mec' — inventář by byl příliš těžký.`. | |
| 2 | Zobrazte inventář. | `inventar` | Příkaz přehledně vypíše aktuální předměty, jejich váhu a statistiky v poměru k max. nosnosti (např. `12/15 váha`). | |

<br><br>

### SPSE Jecna Test Case: MVP12
| Test Case ID: MVP12 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Rozhovor s NPC | |
| **Brief description:** Interakce s postavami herního světa. | |
| **Pre-conditions:** Hráč je v místnosti s NPC (Vstupní hala - Strážce Aldric). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkuste mluvit s neexistujícím NPC. | `mluv karel` | `Žádná postava jménem 'karel' tu není.` | |
| 2 | Promluvte s existujícím NPC. | `mluv aldric` | Zobrazí se ASCII portrét a jedna z náhodných replik postavy v uvozovkách (např. `Strážce Aldric říká: "Vítej..."`). | |

<br><br>

### SPSE Jecna Test Case: MVP13
| Test Case ID: MVP13 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Zobrazení ostatních hráčů v místnosti a připojení více klientů | |
| **Brief description:** Ověření mutliplayer viditelnosti. | |
| **Pre-conditions:** Spuštěni dva klienti. Účty `tester_01` a `tester_02`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | V prvním okně se přihlaste jako `tester_01` a zůstaňte v hale. Ve druhém okně logněte `tester_02`. | Data z předpokladů | Při přihlášení druhého hráče se prvnímu vypíše `>> tester_02 se přihlásil.`. | |
| 2 | V prvním okně napište příkaz. | `prozkoumej` | Výpis místnosti u Klienta 1 zobrazí novou sekci `[HRÁČI] tester_02`. | |

---

## 2. Povinné požadavky (I1–I4, P1)

### SPSE Jecna Test Case: REQ01
| Test Case ID: REQ01 (I1) | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Načítání herního světa z externích souborů | |
| **Brief description:** Změna obsahu světa bez rekompilace programu. | |
| **Pre-conditions:** Server běží. Máte přístup k souborům ve složce `Data/world/`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | V textovém editoru upravte JSON u `vstupni_hala`. Změňte hodnotu `title` a uložte. Poté zapněte server. | `Data/world/rooms.json` (nastavit "Testovaci Hala") | Soubor se uloží. | Server musí být před editací vypnutý. |
| 2 | Připojte se a ověřte stav. | `prozkoumej` | Zobrazí se v hlavičce `╔══ TESTOVACI HALA ══` (změna se projevila). | |

<br><br>

### SPSE Jecna Test Case: REQ02
| Test Case ID: REQ02 (I2) | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Logování na serveru | |
| **Brief description:** Ověření existence a zápisu serverového logu. | |
| **Pre-conditions:** Během testování byly provedeny různé akce (přihlášení, pohyb). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Otevřete logovací soubor na serveru a prohlédněte jej. | `logs/server.log` | Log obsahuje časová razítka a štítky jako `[INFO] [SERVER]`, `[AUTH] [tester_01]` a zaznamenané příkazy. | |

<br><br>

### SPSE Jecna Test Case: REQ03
| Test Case ID: REQ03 (I3) | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Persistence hráče po odpojení | |
| **Brief description:** Ověření zachování lokace a inventáře. | |
| **Pre-conditions:** Účet `tester_01` má u sebe předmět a nenachází se ve Vstupní hale. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Odpojte klienta, znovu ho spusťte a přihlaste se jako `tester_01`. | `/exit` a login | Hráč se po připojení objeví zpět na opuštěném místě a v inventáři mu zůstanou nabyté věci. Stav je zachován. | |

<br><br>

### SPSE Jecna Test Case: REQ04
| Test Case ID: REQ04 (I4) | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Low | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Funkcionalita vlastního klienta | |
| **Brief description:** Test unikátních funkcí vytvořeného konzolového klienta. | |
| **Pre-conditions:** Klient je spuštěn a připojen. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Otestujte historii stisknutím šipky `↑` (Nahoru). | Klávesa `↑` | Šipka nahoru vrátí naposledy zadaný text do vstupního řádku. | |
| 2 | Zadejte klientský příkaz `/clear`. | `/clear` | Příkaz vymaže obsah konzole. | |

<br><br>

### SPSE Jecna Test Case: REQ05
| Test Case ID: REQ05 (P1) | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Dokončení hry (Win condition) | |
| **Brief description:** Test úspěšného dohrání a reakce serveru. | |
| **Pre-conditions:** Účet `hero_99` stojí u win předmětu. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Seberte vítězný předmět příkazem. | `vezmi koruna` (nebo upravená pochodeň) | Zobrazí se ASCII art animace vítězství a text oznamující dokončení hry. | |
| 2 | Ověřte ostatní klienty. | - | Všem hráčům vyskočí globální zpráva `*** hero_99 porazil Temného rytíře a dokončil hru! ***`. | |

---

## 3. Herní mechaniky

### SPSE Jecna Test Case: M_BOJ_01
| Test Case ID: M_BOJ_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Útok na bojové NPC a jeho poražení | |
| **Brief description:** Průchod soubojem a smrtí nepřítele. | |
| **Pre-conditions:** Hráč se nachází v "Hnízdě slizáků" s nepřáteli. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zadejte příkaz k útoku, dokud NPC nepadne. | `utoc blub` | Server zobrazí výpis `⚔ BOJ: tester_01 vs Slizák Blub` a damage log. Při 0 HP server napíše `Slizák Blub je poražen...` a padne loot. | |

<br><br>

### SPSE Jecna Test Case: M_BOJ_02
| Test Case ID: M_BOJ_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Pokus o útok na nebojové NPC | |
| **Brief description:** Test ochrany nevinných NPC proti dmg. | |
| **Pre-conditions:** Hráč je ve Vstupní hale se `Strážcem Aldricem`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zadejte příkaz k útoku na strážce. | `utoc aldric` | Server odmítne útok s hláškou: `Strážce Aldric na tebe smutně hledí...` K boji nedojde. | |

<br><br>

### SPSE Jecna Test Case: M_OBCH_01
| Test Case ID: M_OBCH_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Střední | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Úspěšný nákup u obchodníka | |
| **Brief description:** Nákup zboží pro výměnu zlaťáků za item. | |
| **Pre-conditions:** Hráč je u Kováře Bjorna. Má alespoň 50 zlatých. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zobrazte nabídku a kupte zbraň. | `obchod`, pak `nakup mec` | `obchod` zobrazí ceník. Nákup strhne 50 zlatých, přidá "Meč" a vypíše zprávu `Kupuješ Meč za 50...`. | |

<br><br>

### SPSE Jecna Test Case: M_OBCH_02
| Test Case ID: M_OBCH_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Střední | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Nákup bez dostatku zlatých | |
| **Brief description:** Zajištění proti "ukradení" věci obchodníkovi. | |
| **Pre-conditions:** Hráč je u kováře s méně než 50 zlata. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkuste koupit drahý meč. | `nakup mec` | K nákupu nedojde. Zobrazí se `Nemáš dost zlatých. Potřebuješ 50...`. | |

<br><br>

### SPSE Jecna Test Case: M_STAT_01
| Test Case ID: M_STAT_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Použití předmětu (Léčivý lektvar) | |
| **Brief description:** Aplikace spotřebitelných itemů a hojení HP. | |
| **Pre-conditions:** Hráč utrpěl poškození a má v inventáři `lektvár_leceni` a `mec`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkuste použít zbraň. | `použij mec` | `'Meč' se nedá použít.` | |
| 2 | Použijte lektvar. | `použij lektvár` | Přehrána ASCII animace. Text `Vypiješ lektvar a tvé rány se zacelují.` HP se doplní. | |

<br><br>

### SPSE Jecna Test Case: M_STAT_02
| Test Case ID: M_STAT_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Status efekt místnosti | |
| **Brief description:** Aplikace debuffu z prostředí (jed apod.). | |
| **Pre-conditions:** Místnost má v `rooms.json` nastaven `statusEffect` (poison). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Vstupte do upravené místnosti. Zadejte jakýkoliv další příkaz, aby prošel tah. | `jdi dolu`, `prozkoumej` | Při vstupu server varuje `⚠ Cítíš podivnou energii v místnosti...` a udělí "Otrávení". Po tahu vypíše damage log `[Otrávení] Jed ti koluje v žilách... HP: 95/100`. | |

<br><br>

### SPSE Jecna Test Case: M_SOC_01
| Test Case ID: M_SOC_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Odeslání soukromé zprávy (Whisper) | |
| **Brief description:** Ověření P2P chatu. | |
| **Pre-conditions:** Klient 1 (`tester_01`) a Klient 2 (`tester_02`) jsou připojeni. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | V Klientovi 1 zadejte příkaz k šeptání Klientovi 2. | `šeptej tester_02 Ahoj` | V Klientu 1: `🔒 [Šeptáš hráči tester_02]: Ahoj`. U Klienta 2: `🔒 [tester_01 ti šeptá]: Ahoj`. | |

<br><br>

### SPSE Jecna Test Case: M_SOC_02
| Test Case ID: M_SOC_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Low | **Test Designed date:** 6. 5. 2026 |
| **Test Title:** Pokus o šeptání neexistujícímu/offline hráči | |
| **Brief description:** Reakce příkazu v případě, že socket neexistuje. | |
| **Pre-conditions:** Hráč `tester_01` je připojen. Účet `karel` není připojen. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkuste šeptat offline hráči. | `šeptej karel Tajemství` | `Hráč 'karel' není online.` Zpráva neodejde. | |
| 2 | Zkuste šeptat sami sobě. | `šeptej tester_01 Haló` | `Nemůžeš šeptat sám sobě.` | |
