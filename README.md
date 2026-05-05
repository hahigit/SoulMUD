# SoulMUD — Dokumentace a návod k obsluze

Textová RPG hra pro více hráčů v C# (klient-server architektura). Projekt splňuje požadavky na MVP (Minimum Viable Product) včetně pokročilých mechanik, persistence dat a vlastního klienta.

---

## 1. Jak hru spustit

### Spuštění z vývojového prostředí (Visual Studio / Rider)
1. **Otevření projektu**: Otevřete řešení `.sln` nebo složku s projektem.
2. **Spuštění serveru**: Klikněte pravým tlačítkem na projekt **Server** → *Run* (nebo *Ladit*). Server začne naslouchat na portu 4000.
3. **Spuštění klienta**: Klikněte pravým tlačítkem na projekt **Client** → *Run*.
4. **Více hráčů**: Pro testování interakce stačí spustit projekt **Client** vícekrát.

### Připojení z jiného počítače (stejná Wi-Fi)
1. **IP adresa**: Na PC se serverem zjistěte adresu přes `ipconfig` (např. `192.168.1.50`).
2. **Vlastní klient**: Spusťte klienta příkazem `dotnet run -- <IP_adresa> 4000`.
3. **PuTTY/Telnet**: Připojte se na danou IP a port 4000 (Typ: Telnet nebo Raw).

---

## 2. Technická dokumentace (Specifikace projektu)

### Architektura a síť
* **Server**: Asynchronní zpracování TCP socketů. Každý hráč běží ve vlastním `Tasku`.
* **Data**: Herní svět (místnosti, NPC, předměty) je definován v **JSON** souborech v `Data/world/`. Změna světa nevyžaduje rekompilaci.
* **Logování**: Události se ukládají do `logs/server.log` s časovou značkou.
* **Zabezpečení**: Hesla hráčů jsou zahashována pomocí **BCrypt**.

### Implementované mechaniky
* **M1 Komunikace**: Globální (`křik`), lokální (`řekni`) a soukromý chat (`šeptej`).
* **M2 Souboj s NPC**: Tahový systém s HP, útokem, obranou a respawnem.
* **M4 Obchodování**: Možnost nakupovat a prodávat předměty u NPC obchodníků.
* **M8 Používání předmětů**: Předměty s efekty (léčení, dočasné bonusy, zisk zlata).
* **M12 Stavové efekty**: Systém statusů (otrávení, posílení), které tikají každé kolo.

### Cíl a dokončení hry (P1)
Hra je dokončena poražením Bosse a získáním „Koruny stínu". Vítězství je oznámeno celému serveru a hráč je zapsán do **Síně slávy** (`leaderboard.json`).

---

## 3. Ovládání (Seznam příkazů)

| Kategorie | Příkazy |
| :--- | :--- |
| **Pohyb** | `jdi <směr>`, `prozkoumej` (nebo `l`) |
| **Předměty** | `vezmi <předmět>`, `odlož <předmět>`, `inventář`, `použij <předmět>` |
| **Boj** | `útoč <npc>`, `zdraví` (stav postavy) |
| **Obchod** | `obchod`, `nakup <předmět>`, `prodej <předmět>`, `zlaté` |
| **Komunikace** | `řekni`, `křik`, `šeptej <hráč>`, `online` |
| **Ostatní** | `žebříček`, `pomoc` |

*Poznámka: Server ignoruje diakritiku, můžete psát s háčky i bez nich.*

---

## 4. Testovací scénáře (Test Cases)

Kompletní testovací plán je dokumentován v souboru [`TestCases.md`](./TestCases.md). Zde je přehled jednotlivých kategorií testů:

### MVP Funkce
- [MVP_01: Spuštění serveru](./TestCases.md#spse-jecna-test-case-mvp_01)
- [MVP_02: Připojení klienta](./TestCases.md#spse-jecna-test-case-mvp_02)
- [MVP_03: Neúspěšné přihlášení (neexistující účet)](./TestCases.md#spse-jecna-test-case-mvp_03)
- [MVP_04: Neúspěšné přihlášení (špatné heslo)](./TestCases.md#spse-jecna-test-case-mvp_04)
- [MVP_05: Registrace nového hráče](./TestCases.md#spse-jecna-test-case-mvp_05)
- [MVP_06: Úspěšné přihlášení existujícího hráče](./TestCases.md#spse-jecna-test-case-mvp_06)
- [MVP_07: Příkaz pomoc](./TestCases.md#spse-jecna-test-case-mvp_07)
- [MVP_08: Zobrazení místnosti](./TestCases.md#spse-jecna-test-case-mvp_08)
- [MVP_09: Pohyb mezi místnostmi](./TestCases.md#spse-jecna-test-case-mvp_09)
- [MVP_10: Sebrání a odložení předmětu](./TestCases.md#spse-jecna-test-case-mvp_10)
- [MVP_11: Inventář a překročení kapacity](./TestCases.md#spse-jecna-test-case-mvp_11)
- [MVP_12: Rozhovor s NPC](./TestCases.md#spse-jecna-test-case-mvp_12)
- [MVP_13: Více klientů a zobrazení hráčů v místnosti](./TestCases.md#spse-jecna-test-case-mvp_13)

### Povinné požadavky (I1–I4, P1)
- [REQ_01: Načítání herního světa z externích souborů](./TestCases.md#spse-jecna-test-case-req_01)
- [REQ_02: Logování na serveru](./TestCases.md#spse-jecna-test-case-req_02)
- [REQ_03: Persistence hráče po odpojení](./TestCases.md#spse-jecna-test-case-req_03)
- [REQ_04: Funkcionalita vlastního klienta](./TestCases.md#spse-jecna-test-case-req_04)
- [REQ_05: Dokončení hry (Win condition)](./TestCases.md#spse-jecna-test-case-req_05)

### Herní mechaniky
- [M_BOJ_01: Útok na bojové NPC a jeho poražení](./TestCases.md#spse-jecna-test-case-m_boj_01)
- [M_BOJ_02: Pokus o útok na nebojové NPC](./TestCases.md#spse-jecna-test-case-m_boj_02)
- [M_OBCH_01: Úspěšný nákup u obchodníka](./TestCases.md#spse-jecna-test-case-m_obch_01)
- [M_OBCH_02: Pokus o nákup bez dostatku zlatých](./TestCases.md#spse-jecna-test-case-m_obch_02)
- [M_STAT_01: Použití předmětu (Léčivý lektvar)](./TestCases.md#spse-jecna-test-case-m_stat_01)
- [M_STAT_02: Status efekt místnosti](./TestCases.md#spse-jecna-test-case-m_stat_02)
- [M_SOC_01: Odeslání soukromé zprávy (Whisper)](./TestCases.md#spse-jecna-test-case-m_soc_01)
- [M_SOC_02: Pokus o šeptání neexistujícímu/offline hráči](./TestCases.md#spse-jecna-test-case-m_soc_02)

Všechny test cases jsou také dostupné ve formátech:
- 📄 [TestCases.md](./TestCases.md) — Formát Markdown (doporučeno)
- 📋 [TestCases.pdf](./TestCases.pdf) — Formát PDF
- 📑 [TestCases.docx](./TestCases.docx) — Formát Microsoft Word
