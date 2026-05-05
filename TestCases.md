# Testovací scénáře pro Soul Knight MUD

**Tested Project Name:** Soul Knight MUD

Tento dokument obsahuje sadu testovacích scénářů určených pro manuální testování aplikace. Jsou naformátovány přesně podle **SPŠE Ječná Test Case** šablony tak, aby je mohli provést tví spolužáci.

## Příprava - Testovací účty
Před testováním by si měl tester vytvořit nebo připravit tyto účty (registrace se testuje hned v `MVP_05`):
* **Existující hráč:** Jméno: `test_player`, Heslo: `Test123`
* **Hráč pro registraci:** Jméno: `new_player_001`, Heslo: `New123`
* **Neexistující hráč:** `ghost_player_999`
* **Hráč s uloženým stavem:** `saved_player`, Heslo: `Save123` (používá se pro dokončení hry a test perzistence)

---

## 1. MVP Funkce

### SPSE Jecna Test Case: MVP_01
| Test Case ID: MVP_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Spuštění serveru | |
| **Brief description:** Ověření, že herní server lze úspěšně spustit a naslouchá na daném portu. | |
| **Pre-conditions:** Projekt je načten ve vývojovém prostředí (např. Visual Studio / VS Code) nebo je zkompilován. Port 4000 je volný. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Spustit projekt Server přes terminál (`dotnet run`) nebo zkompilovat a spustit přes IDE. | - | Otevře se konzolové okno a vypíše se zpráva `[SERVER] Herní svět načten...` a `[SERVER] Nasloucháme na portu 4000 (max 20 hráčů)...`. Server nespadne a běží. | Lze použít klávesu **F5** ve vývojovém prostředí. |

<br><br>

### SPSE Jecna Test Case: MVP_02
| Test Case ID: MVP_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Připojení klienta | |
| **Brief description:** Ověření, že se ke spuštěnému serveru dokáže připojit klient. | |
| **Pre-conditions:** Server z testu MVP_01 úspěšně běží. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Spustit vlastního klienta (`dotnet run` ve složce Client) NEBO otevřít telnet klienta jako **PuTTY**. | - | Program se otevře. | V IDE lze použít i F5. |
| 2 | Zadat adresu a port (pouze pro PuTTY, vlastní klient ji má v sobě). | IP: `127.0.0.1`, Port: `4000`, Typ: `Raw` | Klient se připojí, zobrazí uvítací banner "Soul Knight MUD" a vypíše nabídku `[1] Přihlásit se`, `[2] Vytvořit nový účet`. | |

<br><br>

### SPSE Jecna Test Case: MVP_03
| Test Case ID: MVP_03 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Neúspěšné přihlášení (neexistující účet) | |
| **Brief description:** Ověření reakce serveru na pokus o přihlášení účtem, který neexistuje. | |
| **Pre-conditions:** Klient je připojen k serveru. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zvolit možnost přihlášení v uvítací nabídce. | `1` | Server vyzve k zadání jména. | |
| 2 | Zadat jméno účtu, který neexistuje. | `ghost_player_999` | Server vyzve k zadání hesla. | |
| 3 | Zadat libovolné heslo. | `Test1234` | Server vypíše chybu `Špatné jméno nebo heslo.` a znovu zobrazí počáteční nabídku. | |

<br><br>

### SPSE Jecna Test Case: MVP_04
| Test Case ID: MVP_04 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Neúspěšné přihlášení (špatné heslo) | |
| **Brief description:** Ověření reakce serveru na přihlášení existujícím účtem, ale se špatným heslem. | |
| **Pre-conditions:** Zaregistrovaný hráč `test_player` existuje. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zvolit možnost přihlášení v uvítací nabídce. | `1` | Server vyzve k zadání jména. | |
| 2 | Zadat jméno existujícího hráče. | `test_player` | Server vyzve k zadání hesla. | |
| 3 | Zadat nesprávné heslo. | `SpatneHeslo999` | Server vypíše chybu `Špatné jméno nebo heslo.` a znovu zobrazí počáteční nabídku. | |

<br><br>

### SPSE Jecna Test Case: MVP_05
| Test Case ID: MVP_05 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Registrace nového hráče | |
| **Brief description:** Ověření úspěšné registrace a vpuštění hráče do hry. | |
| **Pre-conditions:** Klient je připojen k serveru, účet `new_player_001` ještě neexistuje. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | V nabídce zvolit registraci nového účtu. | `2` | Server vyzve k zadání nového jména. | |
| 2 | Zadat nové, unikátní uživatelské jméno. | `new_player_001` | Server vyzve k zadání hesla. | |
| 3 | Zadat heslo. | `New123` | Vypíše se `Účet 'new_player_001' vytvořen!`. Načte se "Vstupní hala hradu" a objeví se herní prompt. | |

<br><br>

### SPSE Jecna Test Case: MVP_06
| Test Case ID: MVP_06 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Úspěšné přihlášení existujícího hráče | |
| **Brief description:** Ověření loginu u dříve vytvořeného účtu. | |
| **Pre-conditions:** Účet `new_player_001` z testu MVP_05 existuje a hráč není zrovna online. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Připojit se a zvolit `Přihlásit se`. | `1` | Výzva ke jménu. | |
| 2 | Zadat správné jméno a správné heslo. | `new_player_001`, `New123` | Zobrazí se uvítací zpráva `Vítej zpět, new_player_001!`, popis aktuální místnosti a herní prompt. | |

<br><br>

### SPSE Jecna Test Case: MVP_07
| Test Case ID: MVP_07 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Low | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Příkaz pomoc | |
| **Brief description:** Ověření, že hráč má k dispozici nápovědu příkazů. | |
| **Pre-conditions:** Hráč je přihlášen ve hře. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Do herního promptu zadat příkaz k vyvolání nápovědy. | `pomoc` | Zobrazí se formátovaná tabulka rozdělená do sekcí (POHYB, PŘEDMĚTY) obsahující všechny podporované příkazy a jejich syntaxi. | |

<br><br>

### SPSE Jecna Test Case: MVP_08
| Test Case ID: MVP_08 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Zobrazení místnosti | |
| **Brief description:** Ověření funkčnosti příkazu prozkoumej pro výpis okolí. | |
| **Pre-conditions:** Hráč je přihlášen např. v místnosti "Vstupní hala hradu". | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zadat příkaz k prozkoumání okolí. | `prozkoumej` | Vypíše se blok s názvem místnosti, popisem, seznamem dostupných východů (např. `sever`), přítomných NPC (Strážce Aldric) a předmětů. | Alternativně lze testovat `look`. |

<br><br>

### SPSE Jecna Test Case: MVP_09
| Test Case ID: MVP_09 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Pohyb mezi místnostmi | |
| **Brief description:** Ověření, že hráč může chodit mezi lokacemi. | |
| **Pre-conditions:** Hráč je ve Vstupní hale, kde je východ na sever. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkusit jít smyšleným/neplatným směrem. | `jdi zapad` | Server napíše `Směrem 'zapad' se jít nedá.` Hráč zůstává na místě. | |
| 2 | Jít platným směrem. | `jdi sever` | Hráč se přesune do místnosti "Zbrojnice" a automaticky se vypíše její vzhled a obsah. | |

<br><br>

### SPSE Jecna Test Case: MVP_10
| Test Case ID: MVP_10 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Sebrání a odložení předmětu | |
| **Brief description:** Ověření interakce s itemy a úpravy inventáře. | |
| **Pre-conditions:** Hráč je v místnosti s předmětem (ve Vstupní hale je `pochodna`). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkusit sebrat neexistující předmět. | `vezmi nesmysl` | Server napíše `Předmět 'nesmysl' tu nikde nevidíš.` | |
| 2 | Zkusit sebrat existující předmět. | `vezmi pochodna` | Hláška `Vezmeš: pochodna. [Popis předmětu]`. Předmět zmizí z podlahy. | |
| 3 | Předmět následně odložit. | `odlož pochodna` | Hláška `Odložíš pochodna na zem.`. Po zadání `prozkoumej` bude předmět opět ležet v místnosti. | |

<br><br>

### SPSE Jecna Test Case: MVP_11
| Test Case ID: MVP_11 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Inventář a překročení kapacity | |
| **Brief description:** Ověření, že inventář správně sčítá váhu a blokuje přetížení. | |
| **Pre-conditions:** Hráč přešel do Zbrojnice, kde je spousta těžkých předmětů. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Sebrat co nejvíce předmětů, dokud nedojde k překročení limitu (váha 15). | `vezmi mec`, `vezmi stit`, `vezmi helma` | U určitého předmětu server vyhodí hlášku `Nemůžeš vzít '[předmět]' — inventář by byl příliš těžký.` | Limit nosnosti se může lišit dle nastavení. |
| 2 | Vypsat si obsah inventáře. | `inventar` | Vypíše se tabulka držených itemů a ukazatel kapacity např. `14/15 váha`. | |

<br><br>

### SPSE Jecna Test Case: MVP_12
| Test Case ID: MVP_12 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Rozhovor s NPC | |
| **Brief description:** Ověření funkčnosti commandu mluv. | |
| **Pre-conditions:** Hráč je v místnosti s NPC "Strážce Aldric". | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Promluvit na neexistující postavu. | `mluv karel` | Server napíše `Žádná postava jménem 'karel' tu není.` | |
| 2 | Promluvit s přítomným NPC. | `mluv aldric` | Zobrazí se ASCII portrét postavy a jedna z náhodných replik (např. `Strážce Aldric říká: "Vítej, dobrodruhu..."`). | |

<br><br>

### SPSE Jecna Test Case: MVP_13
| Test Case ID: MVP_13 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Více klientů a zobrazení hráčů v místnosti | |
| **Brief description:** Zajištění, že hráči se navzájem vidí ve stejné lokaci. | |
| **Pre-conditions:** Spuštěni dva klienti. Hráč `new_player_001` je online v hale. Spuštěno okno Klienta 2. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Přihlásit hráče č. 2 ze druhého okna klienta. | `test_player`, `Test123` | Klient 1 obdrží broadcast zprávu `>> test_player se přihlásil.` | |
| 2 | V Klientu 1 zadat zobrazení okolí. | `prozkoumej` | Výpis místnosti bude obsahovat novou sekci `[HRÁČI]` s uvedeným jménem `test_player`. | |

---

## 2. Povinné požadavky (I1–I4, P1)

### SPSE Jecna Test Case: REQ_01
| Test Case ID: REQ_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Načítání herního světa z externích souborů | |
| **Brief description:** Ověření vlastnosti I1 – změna JSON souboru se projeví ve hře bez úpravy C#. | |
| **Pre-conditions:** Server je vypnutý. Máte přístup k `Data/world/rooms.json`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Upravit JSON u `vstupni_hala`. Změnit "title" z "Vstupní hala hradu" na "Testovací Hala". | úprava souboru `rooms.json` | Soubor je úspěšně uložen. | |
| 2 | Spustit server a přihlásit hráče. Napsat `prozkoumej`. | `prozkoumej` | Nadpis místnosti bude `╔══ TESTOVACÍ HALA ══`. Data se úspěšně načetla z externího souboru. | Po testu vrátit název zpět! |

<br><br>

### SPSE Jecna Test Case: REQ_02
| Test Case ID: REQ_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Logování na serveru | |
| **Brief description:** Ověření vlastnosti I2 – server zapisuje dění do textového souboru. | |
| **Pre-conditions:** Během testování proběhly nějaké loginy nebo příkazy. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Otevřít složku s logy (obvykle ve složce `logs` nebo `Server/logs`). | Otevřít `server.log` | Soubor je přítomen. | |
| 2 | Prozkoumat obsah log souboru. | - | V souboru jsou zapsány timestampy a štítky typu `[INFO]`, `[AUTH]`, které mapují aktivitu (např. připojení testera, provedení příkazu `jdi`). | |

<br><br>

### SPSE Jecna Test Case: REQ_03
| Test Case ID: REQ_03 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Persistence hráče po odpojení | |
| **Brief description:** Ověření vlastnosti I3 – data se uloží a po restartu nahrají. | |
| **Pre-conditions:** `new_player_001` má u sebe zlaté a např. meč. Nachází se jinde než na startu (např. ve Zbrojnici). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Násilně odpojit klienta nebo napsat příkaz na exit. | Ukončit okno klienta | Server zaloguje odpojení. | |
| 2 | Spustit klienta, znovu se přihlásit. Zkontrolovat stav. | `new_player_001`, `New123`, `prozkoumej`, `inventar` | Hráč nezačíná znovu, ale ocitne se rovnou ve Zbrojnici. Příkaz inventář vypíše vlastněný meč i uložené zlaťáky. | |

<br><br>

### SPSE Jecna Test Case: REQ_04
| Test Case ID: REQ_04 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Low | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Funkcionalita vlastního klienta | |
| **Brief description:** Ověření vlastnosti I4 – specifik klienta proti běžnému telnetu. | |
| **Pre-conditions:** Spuštěn vytvořený `Client`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Provést v rychlosti několik příkazů a poté zmáčknout šipku `↑` (Nahoru). | Klávesa `↑` | Do promptu se vepíše historie – poslední provedený příkaz. | PuTTY tuto fuknci standardně nemá. |
| 2 | Vyzkoušet speciální clear příkaz. | `/clear` | Konzole klienta se celá vymaže, zůstane jen prázdná obrazovka s promptem. | |

<br><br>

### SPSE Jecna Test Case: REQ_05
| Test Case ID: REQ_05 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Dokončení hry (Win condition) | |
| **Brief description:** Ověření vlastnosti P1 – hru je možné úspěšně dohrát ziskem win itemu. | |
| **Pre-conditions:** Účet `saved_player` stojí v místnosti s `Korunou stínu` (Boss room). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Přihlásit se na saved_player a zadat příkaz pro sebrání quest/win předmětu. | Účet: `saved_player`, Heslo: `Save123`, Příkaz `vezmi koruna` | Zobrazí se ASCII art animace vítězství (hvězdy, legendární banner) a gratulační text s časem. | |
| 2 | Ostatní klienti kontrolují obrazovku. | - | Všem hráčům na serveru vyskočí globální zpráva: `*** saved_player porazil Temného rytíře a dokončil hru! ***`. | |

---

## 3. Herní mechaniky

### SPSE Jecna Test Case: M_BOJ_01
| Test Case ID: M_BOJ_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Útok na bojové NPC a jeho poražení | |
| **Brief description:** Ověření funkčnosti stěžejní bojové smyčky. | |
| **Pre-conditions:** Hráč `new_player_001` je v Hnízdě slizáků. Stojí proti `Slizák Blub`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zadat příkaz k napadení nepřítele. | `utoc blub` | Přehraje se bojová ASCII animace slizáka. Poté se vypíše protokol `⚔ BOJ: new_player_001 vs Slizák Blub`. Vypíše se dmg hráče i protiútok nepřítele. | |
| 2 | Pokračovat v útocích do doby, než NPC zemře (HP pod 0). | `utoc blub` x krát | Na konci se objeví hláška o poražení postavy. Vypíše se počet nabytých zlaťáků a na zem spadne loot. | |

<br><br>

### SPSE Jecna Test Case: M_BOJ_02
| Test Case ID: M_BOJ_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Pokus o útok na nebojové NPC | |
| **Brief description:** Ověření, že NPC typu obchodník/spojenec nejde zabít. | |
| **Pre-conditions:** Hráč stojí u Strážce Aldrica ve Vstupní hale. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zadat příkaz k útoku na NPC s isCombatant: false. | `utoc aldric` | Server odmítne útok s hláškou: `Strážce Aldric na tebe smutně hledí. Jsi si jistý, že ho chceš napadnout?` K boji nedojde, nedojde k loss of HP. | |

<br><br>

### SPSE Jecna Test Case: M_OBCH_01
| Test Case ID: M_OBCH_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Úspěšný nákup u obchodníka | |
| **Brief description:** Ověření převodu zlata za itemy. | |
| **Pre-conditions:** Účet `saved_player` je u kováře Bjorna. Z předchozích testů má nastaveno dostatek zlatých (1000). | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zobrazit ceník zboží prodejce. | `obchod` | Vykreslí se ASCII budova obchodu a položky jako Meč, Štít i s jejich cenami. | |
| 2 | Koupit předmět, na který má hráč finance. | `nakup mec` | Server vypíše `Kupuješ Meč za 50 zlatých...`. Úspěšně se předmět přidá do inventáře a zlaťáky se odečtou. | Ceny dle item.json. |

<br><br>

### SPSE Jecna Test Case: M_OBCH_02
| Test Case ID: M_OBCH_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Pokus o nákup bez dostatku zlatých | |
| **Brief description:** Ověření ochrany obchodu proti free itemům. | |
| **Pre-conditions:** Účet pro registraci `new_player_001` je u kováře Bjorna, ale má u sebe jen minimum peněz (např. 10 zlatých). Meč stojí 50. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Napsat příkaz pro koupi nejdražší věci v krámku. | `nakup mec` | K nákupu nedojde. Objeví se chybová hláška `Nemáš dost zlatých. Potřebuješ 50, máš 10.` (hodnoty dle reálných dat). Do inventáře se nic nepřidá. | |

<br><br>

### SPSE Jecna Test Case: M_STAT_01
| Test Case ID: M_STAT_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** High | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Použití předmětu (Léčivý lektvar) | |
| **Brief description:** Interakce a konzumace předmětů ovlivňujících statistiky. | |
| **Pre-conditions:** Hráč má ubráno HP. V inventáři se nachází `lektvár_leceni`. Dále tam leží i obyčejný `mec`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Zkusit použít zbraň nebo běžný item. | `použij mec` | Zobrazí se error `'Meč' se nedá použít.` | |
| 2 | Použít léčivý lektvar. | `použij lektvár` | Přehrána ASCII animace lektvaru. Vypíše se `Vypiješ lektvar a tvé rány se zacelují.` a HP vzroste např. na `100/100`. Item z inventáře zmizí. | |

<br><br>

### SPSE Jecna Test Case: M_STAT_02
| Test Case ID: M_STAT_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Status efekt místnosti | |
| **Brief description:** Ověření, že místnost dokáže hráče ovlivnit environmentálně (např. jedem). | |
| **Pre-conditions:** Místnost (např. Hnízdo slizáků) má v `rooms.json` nastaven parametr `"statusEffect": "poison"`. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Pohyb do toxické místnosti. | `jdi dolu` | Při vstupu server pošle varovnou hlášku: `⚠ Cítíš podivnou energii v místnosti...` a udělí status effect. | |
| 2 | Zadat libovolný command pro prožití jednoho 'tahu'. | `prozkoumej` | Před vypsáním místnosti se aplikuje jed: `[Otrávení] Jed ti koluje v žilách... HP: 95/100` (příklad textu). | |

<br><br>

### SPSE Jecna Test Case: M_SOC_01
| Test Case ID: M_SOC_01 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Medium | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Odeslání soukromé zprávy (Whisper) | |
| **Brief description:** Ověření funkčnosti peer-to-peer chatu bez ohledu na vzdálenost. | |
| **Pre-conditions:** Jsou online klienti `new_player_001` i `test_player`. Každý stojí v jiné místnosti. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | V klientu prvního hráče poslat soukromou zprávu. | `šeptej test_player Ahoj` | Klient 1 uvidí log o úspěšném odeslání: `🔒 [Šeptáš hráči test_player]: Ahoj` | |
| 2 | Zkontrolovat pohled u Klienta 2. | - | U Klienta 2 okamžitě vyskočí zabarvená zpráva `🔒 [new_player_001 ti šeptá]: Ahoj`. | |

<br><br>

### SPSE Jecna Test Case: M_SOC_02
| Test Case ID: M_SOC_02 | Test Designed by: San Nguyen, Hai Hoang |
| :--- | :--- |
| **Test Priority (Low/Medium/High):** Low | **Test Designed date:** 5. 5. 2026 |
| **Test Title:** Pokus o šeptání neexistujícímu/offline hráči | |
| **Brief description:** Ochrana proti zasílání MSG do prázdna a sám sobě. | |
| **Pre-conditions:** Spuštěn pouze hráč `new_player_001`. Účet `karel` je offline. | |

| Step | Test Steps | Test Data | Expected Result | Notes |
| :--- | :--- | :--- | :--- | :--- |
| 1 | Poslat zprávu hráči Karel. | `šeptej karel Test` | Hra ohlásí neúspěch doručení: `Hráč 'karel' není online.` Zpráva se nepošle do globálního chatu. | |
| 2 | Zkusit poslat zprávu na svůj vlastní účet. | `šeptej new_player_001 Haló` | Klient se zachytí a vypíše: `Nemůžeš šeptat sám sobě.` | |
