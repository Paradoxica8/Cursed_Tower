# 🏰 Cursed Tower — Idle Clicker / Incremental RPG

**Cursed Tower** is a classic 2D idle clicker / incremental RPG built in Unity, inspired by the mechanics of *Clicker Heroes* and *Tap Titans 2*.

Players battle monsters within a cursed tower, recruit a squad of unique heroes, upgrade their active click damage, and reset their progress via a Prestige system (the Altar of Ash) to gain meta-progression bonuses.

---

## 🎮 Key Features and Mechanics

### 1. Combat System and Economy
* **Active Gameplay:** Left-clicking on a monster deals "click damage." Accumulated gold is spent to increase the power of these clicks.
* **Floor Progression:** Each floor features 10 standard monsters. The game automatically advances to the next floor upon clearing the current one.
* **Boss Battles:** Every 5th floor contains a boss with increased HP and higher gold rewards, governed by a **30-second timer**. If the player fails to defeat the boss, they are sent back to a previous floor to farm for more strength.
* **Big Numbers:** All mathematical calculations (HP, DPS, Gold) utilize double-precision floating-point numbers (`double`), with automatic formatting for large values ​​(K, M, B, T, Qa, Qi, etc.).

### 2. Hero System in a 2D World
* **Visual Squad:** Recruited heroes appear in the 2D game world, automatically attacking the current enemy once per second. * **Passive Abilities:** Every few levels (10, 25, 50, 100), heroes automatically unlock powerful passive bonuses:
* Multipliers for their own DPS. 
* A team-wide damage multiplier. 
* Bonuses to gold drops.
* **Purchase Toggle (QoL):** A feature to switch between bulk-purchase modes with a single click: `x1 / x10 / x100 / MAX`, with costs automatically calculated using a geometric progression formula.

### 3. Prestige System
* **Altar of Ash:** Upon reaching floor 15 or higher, players can perform a "Prestige" (reset) to earn a meta-currency called **Ancient Ash**.
* **Ancient Talents:** Ash can be spent on permanent, long-term upgrades:
* ⚔️ **Ancestral Strength:** +25% team-wide damage per level. 
* 💰 **Tower Wealth:** +20% gold earned per level. 
* 👆 **Powerful Click:** +30% click damage per level.

### 4. Game Feel & UI
* **Dynamic Floating Damage Text:** Damage from player clicks and hero auto-attacks is visually distinguished by color, scale, and movement trajectory.
* **Event-Driven HUD:** The user interface updates via event subscriptions, avoiding resource-heavy `Update()` polling.
* **Robust Save System:** Progress is automatically saved in JSON format (via PlayerPrefs) every 10 seconds, with built-in crash protection. ---

## 🛠️ Technical Stack

* **Engine:** Unity 2D (URP / Universal Render Pipeline).
* **Language:** C#.
* **Architecture:** Event-Driven Architecture (`GameEvents.cs`), Singleton pattern for system managers.
* **UI:** TextMeshPro, Vertical Layout Groups, Content Size Fitter, Canvas Scaler.
* **Data-Driven Design:** Hero data is stored in ScriptableObjects (`HeroData.cs`).

---

## 📁 Project Structure

text
Assets/_Scripts/
├── Core/              # Number formatting, GameEvents, cost calculations
├── Characters/        # Hero logic and in-scene animations
├── Enemies/           # Enemy and boss logic
├── Data/              # ScriptableObjects for hero parameter configuration
├── Systems/           # Managers: Tower, Gold, Click, Hero, Prestige, Save, DamageText
└── UI/                # UI components (HUD, HeroCard, PrestigeUI, BuyToggle)

-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

# 🏰 Cursed Tower — Idle Clicker / Incremental RPG

**Cursed Tower** — это классический 2D Idle Clicker / Incremental RPG на Unity, вдохновленный механиками *Clicker Heroes* и *Tap Titans 2*.

Игрок сражается с монстрами в проклятой башне, нанимает отряд уникальных героев, прокачивает активный урон клика и сбрасывает прогресс через систему Престижа (Алтарь Пепла) для получения мета-усилений.

---

## 🎮 Ключевые Фичи и Механики

### 1. Боевая Система и Экономика
* **Активный Геймплей:** Левый клик по монстру наносит урон клика. Накопленное золото тратится на прокачку силы клика.
* **Прогрессия Этажей:** 10 обычных монстров на этаже. Переход на следующий этаж происходящим автоматически.
* **Битвы с Боссами:** Каждый 5-й этаж содержит Босса с увеличенным запасом HP, повышенной наградой в золоте и **30-секундным таймером**. В случае поражения игрок откатывается на обычный этаж для дофарма.
* **Big Numbers:** Все математические расчеты (HP, DPS, Золото) работают на численных типах двойной точности (`double`) с автоматическим сжатием больших чисел (K, M, B, T, Qa, Qi...).

### 2. Система Героев в 2D-Мире
* **Визуальный Отряд:** Нанятые герои выстраиваются на сцене в 2D-мире и автоматически атакуют текущего врага раз в секунду.
* **Пассивные Способности:** Каждые несколько уровней (10, 25, 50, 100) герои автоматически разблокируют мощные пассивные бонусы:
  * Множители собственного DPS.
  * Командный множитель урона.
  * Бонусы к выпадению золота.
* **Переключатель Покупок (QoL):** Возможность переключения режимов массовой покупки за 1 клик: `x1 / x10 / x100 / MAX` с автоматическим математическим расчетом стоимости по формуле геометрической прогрессии.

### 3. Система Перерождения (Prestige)
* **Алтарь Пепла:** Достигнув 15+ этажа, игрок может выполнить Сброс (Prestige) и получить мета-валюту — **Древний Пепел (Ash)**.
* **Древние Таланты:** Пепел тратится на бессрочные перманентные апгрейды:
  * ⚔️ **Сила Предков:** +25% к урону всей команды за уровень.
  * 💰 **Богатство Башни:** +20% к получаемому золоту за уровень.
  * 👆 **Мощный Клик:** +30% к урону клика за уровень.

### 4. Game Feel & UI
* **Динамический Вылетающий Урон (Floating Damage Text):** Удары игрок и авто-атаки героев визуально различаются цветом, масштабом и траекторией полета.
* **Событийно-Ориентированный HUD:** Интерфейс обновляется через подписки на события без использования тяжелых `Update()` опросов.
* **Надежные Сохранения:** Автоматическая запись прогресса в формате JSON (PlayerPrefs) каждые 10 секунд с защитой от сбоев.

---

## 🛠️ Технический Стек

* **Движок:** Unity 2D (URP / Universal Render Pipeline).
* **Язык:** C#.
* **Архитектура:** Event-Driven Architecture (`GameEvents.cs`), Singleton Pattern для менеджеров систем.
* **UI:** TextMeshPro, Vertical Layout Groups, Content Size Fitter, Canvas Scaler.
* **Data-Driven Design:** Данные героев вынесены в ScriptableObjects (`HeroData.cs`).

---

## 📁 Структура Проекта

text
Assets/_Scripts/
├── Core/              # Форматирование чисел, событие GameEvents, математика цен
├── Characters/        # Логика и анимация героев на сцене
├── Enemies/           # Логика врагов и боссов
├── Data/              # ScriptableObjects для настройки параметров героев
├── Systems/           # Менеджеры: Tower, Gold, Click, Hero, Prestige, Save, DamageText
└── UI/                # Компоненты интерфейса (HUD, HeroCard, PrestigeUI, BuyToggle)
