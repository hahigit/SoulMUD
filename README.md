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
Hra je dokončena poražením Bosse a získáním „Koruny stínu“. Vítězství je oznámeno celému serveru a hráč je zapsán do **Síně slávy** (`leaderboard.json`).

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
