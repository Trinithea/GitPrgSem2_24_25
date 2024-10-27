# Zpětná vazba pro pana Tulacha

## Herní mechanika

### Pozitiva
- **Zajímavý nápad s odlišením pohlaví a jejich vlastností:**
  - Žena má víc HP, ale začíná s méně penězi.
  - Při pohybu získává žena životy, zatímco muž peníze.
  - Muž ztrácí více peněz při útěku, ale neztrácí životy.

- **Přehledný user interface:** 
  - Vždy je vypsán pokyn a na dalším řádku je místo pro user input.

### Negativa
- **Chybí zobrazení mapy:** 
  - Hráč neví, kam se pohybuje a kde jsou oponenti, takže musí chodit náhodně dokola.
  
- **Chybí input checky:** 
  - Nikde není kontrolováno, zdali hráč zadává správný input (může napsat cokoliv a hra mu dá buď zdarma životy a peníze, nebo to crashne).

---

## Kód

### Pozitiva
- **Použil dědičnost.** 
  
- **Použil abstrakci.**

### Negativa
- **Chybí třída pro hlavní herní smyčku:** 
  - Chybí třída **Game** nebo něco podobného, která by schovala hlavní game loop a inicializaci hry (teď je to všechno narvané v mainu).

- **Špatné použití `virtual` a `abstract`:**
  - V abstraktní třídě **Hrac** jsou metody **Pohyb** a **Utek** deklarovány jako `virtual`. Jelikož neposkytují žádnou implementaci, bylo by lepší použít `abstract` (nebo rovnou `interface`).

- **Nepoužil rozhraní.** 
  - Nevím, jestli to bylo povinné podle zadání, nebo ne.

---

### Celkové hodnocení
**140/150**
