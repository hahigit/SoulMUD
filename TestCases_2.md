# Testovací scénáře pro Soul Knight MUD

Tento dokument obsahuje sadu testovacích scénářů (Test Cases) určených pro manuální testování aplikace Soul Knight MUD. Testy jsou napsány tak, aby je mohl provést kdokoli bez předchozí znalosti kódu.

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

### MVP01: Spuštění serveru
* **Priorita:** Vysoká
* **Předpoklady:** Aplikace je zkompilována. Port 4000 je volný.
* **Kroky:**
  1. Otevřete terminál ve složce `Server`.
  2. Spusťte příkaz `dotnet run`.
* **Testovací data:** Žádná.
* **Očekávaný výsledek:** V konzoli serveru se vypíše zpráva `[SERVER] Herní svět načten...` a `[SERVER] Nasloucháme na portu 4000 (max 20 hráčů)...`. Server nespadne a běží.

### MVP02: Připojení klienta
* **Priorita:** Vysoká
* **Předpoklady:** Server běží podle MVP01.
* **Kroky:**
  1. Otevřete nový terminál ve složce `Client`.
  2. Spusťte příkaz `dotnet run`.
* **Testovací data:** IP: `localhost`, Port: `4000` (výchozí v klientovi).
* **Očekávaný výsledek:** Klient se připojí, zobrazí uvítací banner hry s textem "Soul Knight MUD" a vypíše nabídku k přihlášení (`[1] Přihlásit se`, `[2] Vytvořit nový účet`).

### MVP03: Neúspěšné přihlášení (neexistující účet)
* **Priorita:** Vysoká
* **Předpoklady:** Klient je připojen k serveru.
* **Kroky:**
  1. Zvolte `1` (Přihlásit se).
  2. Zadejte jméno neexistujícího hráče.
  3. Zadejte libovolné heslo.
* **Testovací data:** Jméno: `ghost_player_999`, Heslo: `Cokoliv`.
* **Očekávaný výsledek:** Server zobrazí zprávu `Špatné jméno nebo heslo.` a znovu zobrazí úvodní nabídku pro přihlášení/registraci.

### MVP04: Neúspěšné přihlášení (špatné heslo)
* **Priorita:** Vysoká
* **Předpoklady:** Existuje hráč `tester_02`.
* **Kroky:**
  1. Zvolte `1` (Přihlásit se).
  2. Zadejte jméno existujícího hráče.
  3. Zadejte **nesprávné** heslo.
* **Testovací data:** Jméno: `tester_02`, Heslo: `SpatneHeslo999`.
* **Očekávaný výsledek:** Server zobrazí zprávu `Špatné jméno nebo heslo.` a znovu zobrazí úvodní nabídku.

### MVP05: Registrace nového hráče
* **Priorita:** Vysoká
* **Předpoklady:** Klient je připojen, účet `tester_01` ještě neexistuje.
* **Kroky:**
  1. V nabídce zvolte `2` (Vytvořit nový účet).
  2. Zadejte nové uživatelské jméno.
  3. Zadejte heslo.
* **Testovací data:** Volba: `2`, Jméno: `tester_01`, Heslo: `Heslo123`.
* **Očekávaný výsledek:** Zobrazí se zpráva `Účet 'tester_01' vytvořen! Dobrodružství začíná...`. Následně se automaticky načte první místnost ("Vstupní hala hradu") a zobrazí se herní prompt `[tester_01 | HP:100]>`.

### MVP06: Úspěšné přihlášení existujícího hráče
* **Priorita:** Vysoká
* **Předpoklady:** Účet `tester_01` existuje (vytvořen v MVP05). Hráč aktuálně není připojen.
* **Kroky:**
  1. Spusťte klienta a zvolte `1` (Přihlásit se).
  2. Zadejte správné uživatelské jméno a heslo.
* **Testovací data:** Jméno: `tester_01`, Heslo: `Heslo123`.
* **Očekávaný výsledek:** Zobrazí se uvítací zpráva `Vítej zpět, tester_01!` a vypíše se obsah aktuální místnosti. Hráč se ocitne v herní smyčce.

### MVP07: Příkaz pomoc
* **Priorita:** Nízká
* **Předpoklady:** Hráč je přihlášen ve hře.
* **Kroky:**
  1. Do herního promptu zadejte příkaz pro nápovědu.
* **Testovací data:** Příkaz: `pomoc`.
* **Očekávaný výsledek:** Zobrazí se strukturovaná tabulka s nápovědou rozdělená do kategorií (POHYB, PŘEDMĚTY, POSTAVY & BOJ, atd.) obsahující syntaxi příkazů (např. `jdi <směr>`).

### MVP08: Zobrazení místnosti (prozkoumej)
* **Priorita:** Střední
* **Předpoklady:** Hráč je přihlášen (např. v místnosti Vstupní hala hradu).
* **Kroky:**
  1. Zadejte příkaz k prozkoumání místnosti.
* **Testovací data:** Příkaz: `prozkoumej` (nebo `look`).
* **Očekávaný výsledek:** Vypíše se ohraničený blok s názvem místnosti, jejím popisem, dostupnými východy (např. `sever`, `dolu`), seznamem předmětů a přítomnými NPC (např. `Strážce Aldric`).

### MVP09: Pohyb mezi místnostmi
* **Priorita:** Vysoká
* **Předpoklady:** Hráč je ve Vstupní hale, kde je východ na sever.
* **Kroky:**
  1. Zkuste jít neplatným směrem.
  2. Zkuste jít platným směrem (sever).
* **Testovací data:**
  * Neplatný směr: `jdi zapad`
  * Platný směr: `jdi sever`
* **Očekávaný výsledek:**
  * Při `jdi zapad` server odpoví `Směrem 'zapad' se jít nedá.`.
  * Při `jdi sever` se hráč přesune do místnosti "Zbrojnice" a automaticky se vypíše její popis a obsah.

### MVP10: Sebrání a odložení předmětu
* **Priorita:** Vysoká
* **Předpoklady:** Hráč je v místnosti s předmětem (ve Vstupní hale je `pochodna`).
* **Kroky:**
  1. Zkuste sebrat neexistující předmět.
  2. Zkuste sebrat existující předmět.
  3. Předmět následně odložte.
* **Testovací data:**
  * Neexistující: `vezmi blbost`
  * Existující: `vezmi pochodna`
  * Odložení: `odlož pochodna`
* **Očekávaný výsledek:**
  * `vezmi blbost` -> `Předmět 'blbost' tu nikde nevidíš.`
  * `vezmi pochodna` -> `Vezmeš: pochodna. [Popis předmětu]` a předmět zmizí z místnosti.
  * `odlož pochodna` -> `Odložíš pochodna na zem.` a předmět se znovu objeví v místnosti (viditelné po příkazu `prozkoumej`).

### MVP11: Inventář a překročení kapacity
* **Priorita:** Střední
* **Předpoklady:** Hráč má ve Vstupní hale předměty, jejichž celková váha přesahuje kapacitu (15).
* **Kroky:**
  1. Seberte těžké předměty ze Zbrojnice (přesuňte se přes `jdi sever`). Zkuste sebrat vše (`mec`, `stit`, atd.), dokud nenarazíte na limit.
  2. Zobrazte inventář.
* **Testovací data:** Příkazy `vezmi mec`, `vezmi stit`, `inventar`.
* **Očekávaný výsledek:**
  * Při překročení kapacity server vypíše např. `Nemůžeš vzít 'mec' — inventář by byl příliš těžký.`.
  * Příkaz `inventar` přehledně vypíše aktuální předměty, jejich váhu a statistiky v poměru k maximální nosnosti (např. `12/15 váha`).

### MVP12: Rozhovor s NPC
* **Priorita:** Střední
* **Předpoklady:** Hráč je v místnosti s NPC (Vstupní hala - Strážce Aldric).
* **Kroky:**
  1. Zkuste mluvit s neexistujícím NPC.
  2. Promluvte s existujícím NPC.
* **Testovací data:**
  * Neexistující: `mluv karel`
  * Existující: `mluv aldric`
* **Očekávaný výsledek:**
  * `mluv karel` -> `Žádná postava jménem 'karel' tu není.`
  * `mluv aldric` -> Zobrazí se ASCII portrét a jedna z náhodných replik postavy v uvozovkách (např. `Strážce Aldric říká: "Vítej, dobrodruhu..."`).

### MVP13: Zobrazení ostatních hráčů v místnosti a připojení více klientů
* **Priorita:** Vysoká
* **Předpoklady:** Spuštěni dva klienti. Účty `tester_01` a `tester_02`.
* **Kroky:**
  1. V prvním okně se přihlaste jako `tester_01` a zůstaňte ve Vstupní hale.
  2. Ve druhém okně se přihlaste jako `tester_02`.
  3. V prvním okně (`tester_01`) napište `prozkoumej`.
* **Testovací data:** Příkaz `prozkoumej` za `tester_01`.
* **Očekávaný výsledek:**
  * Při přihlášení druhého hráče se prvnímu hráči vypíše `>> tester_02 se přihlásil.`.
  * Příkaz `prozkoumej` u `tester_01` ve výpisu místnosti zobrazí novou sekci `[HRÁČI] tester_02`.

---

## 2. Povinné požadavky (I1–I4, P1)

### REQ01 (I1): Načítání herního světa z externích souborů
* **Priorita:** Střední
* **Předpoklady:** Server běží. Máte přístup k souborům ve složce `Data/world/`.
* **Kroky:**
  1. Odpojte klienty a vypněte server (`Ctrl+C`).
  2. V textovém editoru upravte soubor `Data/world/rooms.json` u místnosti s id `vstupni_hala`. Změňte hodnotu `title` na `Testovaci Hala`.
  3. Zapněte server a připojte klienta.
* **Testovací data:** Úprava JSONu, příkaz `prozkoumej`.
* **Očekávaný výsledek:** Příkaz `prozkoumej` po přihlášení zobrazí v hlavičce `╔══ TESTOVACI HALA ══` (změna se projevila bez úpravy C# kódu). Následně vraťte změnu zpět.

### REQ02 (I2): Logování na serveru
* **Priorita:** Střední
* **Předpoklady:** Během testování byly provedeny různé akce (přihlášení, pohyb, chyby).
* **Kroky:**
  1. Otevřete soubor `logs/server.log` (ve složce Serveru).
  2. Projděte obsah logu.
* **Testovací data:** Soubor `server.log`.
* **Očekávaný výsledek:** Log obsahuje časová razítka a štítky. Záznamy ukazují např. `[INFO] [SERVER] Server spuštěn.`, `[AUTH] [tester_01] Nový účet vytvořen.` a zaznamenané příkazy hráčů.

### REQ03 (I3): Persistence hráče po odpojení
* **Priorita:** Vysoká
* **Předpoklady:** Účet `tester_01` má u sebe zlaté a předmět (např. pochodeň) a nenachází se ve Vstupní hale (přešel např. do Zbrojnice).
* **Kroky:**
  1. V klientovi zadejte příkaz pro odpojení.
  2. Klienta znovu spusťte, přihlaste se jako `tester_01`.
  3. Zkontrolujte polohu a inventář.
* **Testovací data:** Příkazy `/exit`, `prozkoumej`, `inventar`.
* **Očekávaný výsledek:** Hráč se po novém připojení objeví zpět ve Zbrojnici (ne ve Vstupní hale) a v inventáři mu zůstane pochodeň a zlaté. Stav je zachován.

### REQ04 (I4): Funkcionalita vlastního klienta
* **Priorita:** Nízká
* **Předpoklady:** Klient je spuštěn a připojen.
* **Kroky:**
  1. Otestujte historii zadáním několika příkazů a následným stisknutím šipky `↑` (Nahoru).
  2. Zadejte klientský příkaz `/clear`.
* **Testovací data:** Šipka `↑`, příkaz `/clear`.
* **Očekávaný výsledek:** Šipka nahoru vrátí naposledy zadaný text do vstupního řádku. Příkaz `/clear` vymaže obsah konzole (obrazovky).

### REQ05 (P1): Dokončení hry (Win condition)
* **Priorita:** Vysoká
* **Předpoklady:** Založte účet `hero_99`. Server běží. (Pro usnadnění můžete v `items.json` nastavit `isWinCondition: true` u předmětu `pochodna` ve Vstupní hale).
* **Kroky:**
  1. Jděte k předmětu definujícímu výhru (Koruna stínu v Boss komnatě, nebo upravená pochodeň).
  2. Předmět seberte příkazem `vezmi`.
* **Testovací data:** Příkaz `vezmi koruna` (nebo `vezmi pochodna`).
* **Očekávaný výsledek:** Zobrazí se ASCII art animace vítězství (hvězdy a banner) a text oznamující dokončení hry. Všem připojeným hráčům server pošle globální zprávu `*** hero_99 porazil Temného rytíře a dokončil hru! ***`.

---

## 3. Herní mechaniky

### M_BOJ_01: Útok na bojové NPC a jeho poražení
* **Priorita:** Vysoká
* **Předpoklady:** Hráč se nachází v "Hnízdě slizáků" (dostupné směrem dolů ze Vstupní haly). Je zde `slizak_blub`.
* **Kroky:**
  1. Zadejte příkaz k útoku.
  2. Pokračujte v útoku, dokud NPC nepadne.
* **Testovací data:** Příkaz `utoc blub`.
* **Očekávaný výsledek:** Server zobrazí bojový výpis `⚔ BOJ: tester_01 vs Slizák Blub` s uděleným a obdrženým poškozením. Zobrazí se ASCII animace slizáka. Při dosažení 0 HP u NPC server napíše `Slizák Blub je poražen! Získáváš 8 zlatých.` a do místnosti spadne předmět "Zelený sliz".

### M_BOJ_02: Pokus o útok na nebojové NPC
* **Priorita:** Střední
* **Předpoklady:** Hráč je ve Vstupní hale se `Strážcem Aldricem`.
* **Kroky:**
  1. Zadejte příkaz k útoku na strážce.
* **Testovací data:** Příkaz `utoc aldric`.
* **Očekávaný výsledek:** Server odmítne útok s hláškou: `Strážce Aldric na tebe smutně hledí. Jsi si jistý, že ho chceš napadnout?` K boji nedojde.

### M_OBCH_01: Úspěšný nákup u obchodníka
* **Priorita:** Střední
* **Předpoklady:** Hráč je ve Zbrojnici (na sever od Haly) u Kováře Bjorna. Hráč má alespoň 50 zlatých.
* **Kroky:**
  1. Zobrazte nabídku obchodu.
  2. Kupte předmět.
* **Testovací data:**
  * Zobrazení: `obchod`
  * Nákup: `nakup mec`
* **Očekávaný výsledek:** Příkaz `obchod` zobrazí ASCII art obchodu a ceník. Příkaz `nakup mec` strhne hráči 50 zlatých, přidá "Meč" do inventáře a vypíše zprávu `Kupuješ Meč za 50 zlatých...`.

### M_OBCH_02: Nákup bez dostatku zlatých
* **Priorita:** Střední
* **Předpoklady:** Hráč je u kováře Bjorna, ale má méně než 50 zlatých (např. nově vytvořený účet má 10).
* **Kroky:**
  1. Zkuste koupit meč.
* **Testovací data:** Příkaz `nakup mec`.
* **Očekávaný výsledek:** K nákupu nedojde. Server odpoví: `Nemáš dost zlatých. Potřebuješ 50, máš 10.` (hodnoty se mohou lišit podle aktuálního stavu).

### M_STAT_01: Použití předmětu (Léčivý lektvar)
* **Priorita:** Vysoká
* **Předpoklady:** Hráč utrpěl poškození v boji (HP je menší než maximum) a má v inventáři `lektvár_leceni` (možno koupit u Lyry).
* **Kroky:**
  1. Zkuste použít předmět, který nelze použít (např. meč).
  2. Použijte lektvar.
* **Testovací data:** `použij mec`, `použij lektvár`.
* **Očekávaný výsledek:**
  * U meče: `'Meč' se nedá použít.`
  * U lektvaru: Zobrazí se ASCII animace lahvičky, text `Vypiješ lektvar a tvé rány se zacelují.` a HP hráče se doplní (např. `HP: 100/100`). Předmět z inventáře zmizí.

### M_STAT_02: Status efekt místnosti
* **Priorita:** Střední
* **Předpoklady:** V souboru `rooms.json` má místnost nastaven `statusEffect` (např. "poison"). V defaultních datech může chybět, pro test přidejte `"statusEffect": "poison"` např. do "Hnízda slizáků".
* **Kroky:**
  1. Vstupte do upravené místnosti.
  2. Napište jakýkoliv další příkaz (např. `prozkoumej`), aby se "odtickoval" čas.
* **Testovací data:** Přechod do místnosti `jdi dolu` (pokud je to hnízdo). Příkazy v místnosti.
* **Očekávaný výsledek:** Při vstupu server červeně varuje `⚠ Cítíš podivnou energii v místnosti...` a aplikuje status "Otrávení". Při zadání dalšího příkazu server vypíše poškození jedem `[Otrávení] Jed ti koluje v žilách... HP: 95/100`.

### M_SOC_01: Odeslání soukromé zprávy (Whisper)
* **Priorita:** Střední
* **Předpoklady:** Klient 1 (`tester_01`) a Klient 2 (`tester_02`) jsou připojeni. Nemusí být ve stejné místnosti.
* **Kroky:**
  1. V Klientovi 1 zadejte příkaz k šeptání Klientovi 2.
* **Testovací data:** Příkaz (v Klient 1): `šeptej tester_02 Ahoj, kde jsi?`
* **Očekávaný výsledek:**
  * V Klientovi 1 se zobrazí: `🔒 [Šeptáš hráči tester_02]: Ahoj, kde jsi?`
  * V Klientovi 2 se zobrazí fialovým textem: `🔒 [tester_01 ti šeptá]: Ahoj, kde jsi?`

### M_SOC_02: Pokus o šeptání neexistujícímu/offline hráči
* **Priorita:** Střední
* **Předpoklady:** Hráč `tester_01` je připojen. Účet `karel` není připojen.
* **Kroky:**
  1. Zkuste šeptat hráči, který není online.
  2. Zkuste šeptat sami sobě.
* **Testovací data:**
  * Offline hráč: `šeptej karel Tajemství`
  * Sám sobě: `šeptej tester_01 Haló`
* **Očekávaný výsledek:**
  * Při psaní offline hráči: `Hráč 'karel' není online.`
  * Při psaní sobě: `Nemůžeš šeptat sám sobě.` Zpráva se nikam neodešle.
