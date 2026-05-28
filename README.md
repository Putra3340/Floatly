<p align="center">
  <img src="Assets/wpfui-icon-256.png" alt="Floatly Logo" width="120" />
</p>

<h1 align="center">Floatly</h1>

<p align="center">
  <strong>A modern, lightweight media player with a floating Picture-in-Picture interface for Windows.</strong>
</p>

<p align="center">
  <a href="https://dotnet.microsoft.com/download/dotnet/10.0"><img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10" /></a>
  <a href="https://github.com/lepoco/wpfui"><img src="https://img.shields.io/badge/WPF--UI-4.3.0-0078D7?style=for-the-badge&logo=windows&logoColor=white" alt="WPF-UI" /></a>
  <a href="https://learn.microsoft.com/en-us/dotnet/desktop/wpf/"><img src="https://img.shields.io/badge/WPF-Desktop-68217A?style=for-the-badge&logo=windows&logoColor=white" alt="WPF" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="License" /></a>
</p>

<p align="center">
  <a href="#features">Features</a> •
  <a href="#architecture">Architecture</a> •
  <a href="#technology-stack">Tech Stack</a> •
  <a href="#getting-started">Getting Started</a> •
  <a href="#project-structure">Project Structure</a> •
  <a href="#contributing">Contributing</a>
</p>

---

## About

**Floatly** is a modern, feature-rich media player for Windows built with **.NET 10** and **WPF-UI (Fluent Design)**. It supports both video and audio playback with a unique **floating Picture-in-Picture (PiP) window** that stays on top of your workspace — complete with synchronized captions, playback controls, and drag-anywhere convenience.

The project has been **fully upgraded and continued** from its original codebase to a modern architecture using the **MVVM pattern**, **dependency injection** via the .NET Generic Host, **CommunityToolkit.Mvvm** for reactive bindings, and **WPF-UI 4.3** for a native Windows 11 Fluent Design experience. A single `MediaElement` is shared between windows via a reparenting technique, ensuring seamless, uninterrupted playback when switching between the main window and PiP.

---

## Features

### 🎵 Core Playback
- **Floating PiP Window** — A compact, borderless, always-on-top mini-player that renders video with a blurred thumbnail backdrop, playback controls, and a seek bar. Drag it anywhere on your screen.
- **Full Main Window** — A Fluent Design navigation window with Mica backdrop, left-side navigation pane, page routing, and a persistent bottom playback bar.
- **Shared MediaElement** — A single `GlobalPlayerControl` is physically reparented between the main window and PiP window, ensuring zero playback interruption when switching views.
- **Play / Pause / Skip ±10s** — Transport controls available in both windows.
- **Seek Bar** — Precise track seeking with real-time position updates via a `DispatcherTimer` (500ms tick).
- **Volume Control** — Adjustable volume slider with percentage display.

### 📝 Synchronized Captions
- **SRT Caption Syncing** — Full SRT subtitle parser built into the `PlayerService`. Captions are matched to the current playback position on every timer tick, displaying the active lyric line in real time.
- **Multi-Language Support** — Each track can have captions in multiple languages, selectable via the SlideMenu.
- **Slide Menu Panel** — A custom animated slide-in/slide-out panel (250ms `CubicEase` easing) for selecting caption languages or turning captions off. Features configurable width, overlay dimming, and smooth `TranslateTransform` animation.

### 🎨 Theming & Personalization
- **Dark / Light Mode** — Full theme switching powered by WPF-UI's `ApplicationThemeManager`.
- **Accent Colors** — Choose from three curated accent colors: **Indigo** (`#6366f1`), **Deep Indigo** (`#4f46e5`), and **Purple** (`#A273FF`).
- **Fluent Design** — Native Windows 11 look and feel with **Mica backdrop**, rounded corners, and modern iconography via `SymbolRegular` icons.
- **Per-Monitor DPI Awareness** — Configured via the app manifest for crisp rendering on high-DPI displays.

### 🔍 Search & Discovery
- **Content Search** — Search the Floatly server catalog with results showing thumbnail, title, uploader, and play count. Triggered via Enter key or search button.
- **Data Visualization** — A demo page rendering 8,192 virtualized color tiles using `VirtualizingItemsControl` for performance testing.

### 🏗️ Technical Highlights
- **MVVM Architecture** — Clean separation of concerns using Views, ViewModels, Models, and Services with CommunityToolkit.Mvvm source generators.
- **Dependency Injection** — All services, ViewModels, and windows are registered and resolved via `Microsoft.Extensions.Hosting`.
- **Event-Driven State** — `PlayerService.StateChanged` event propagates playback state (position, duration, playing status, active lyric) to all subscribed ViewModels on every tick.
- **Service Abstraction** — All services implement interfaces (`IApiService`, `IPlayerService`) for testability and loose coupling.
- **Hosted Service Lifecycle** — `ApplicationHostService` manages bootstrap and navigation via `IHostedService`.

---

## Architecture

Floatly follows a **layered MVVM architecture** with dependency injection at its core:

```
┌─────────────────────────────────────────────────────────┐
│                      Application                        │
│  App.xaml.cs — .NET Generic Host + DI Container         │
├─────────────────────────────────────────────────────────┤
│                        Views                            │
│  Windows/  │  Pages/       │  Controls/                 │
│  ─────────  ──────────────  ──────────────────          │
│  MainWindow │  HomePage     │  GlobalPlayerControl       │
│  Floating   │  DataPage     │  SlideMenu                │
│  Window     │  SettingsPage │                            │
├─────────────────────────────────────────────────────────┤
│                     ViewModels                          │
│  MainWindowViewModel  │  FloatingWindowViewModel        │
│  HomeViewModel        │  DataViewModel                  │
│  SettingsViewModel    │                                 │
├─────────────────────────────────────────────────────────┤
│                      Services                           │
│  PlayerService (IPlayerService)                         │
│  ApiService (IApiService)                               │
│  ApplicationHostService                                 │
├─────────────────────────────────────────────────────────┤
│                       Models                            │
│  Song  │  Lyrics  │  Search  │  AppConfig               │
│  DataColor  │  MenuItem  │  SongMessage                 │
├─────────────────────────────────────────────────────────┤
│                Helpers / Enums                          │
│  ThemeHelper  │  EnumToBooleanConverter                  │
│  ThemeAccent (enum)                                     │
└─────────────────────────────────────────────────────────┘
```

### Data Flow

```
User Action → View → ViewModel (Command) → Service → API/Backend
                ↑                                        ↓
                └──── Property Changed ← Model Update ←─┘
```

### State Propagation

ViewModels synchronize via the `PlayerService.StateChanged` event:

```
PlayerService ─── StateChanged Event (500ms tick) ───┬──→ MainWindowViewModel
     │                                               └──→ FloatingWindowViewModel
     │
     ├── Updates: Position, Duration, IsPlaying, ActiveLyric
     └── Triggers: OnPropertyChanged for all bound properties
```

### MediaElement Reparenting

A single `GlobalPlayerControl` is physically moved between windows:

```
MainWindow.PlayerHost ←──── GlobalPlayerControl ────→ FloatingWindow.PlayerHost
         (on focus)          (shared MediaElement)         (on blur)
```

---

## Technology Stack

| Category | Technology | Version |
|---|---|---|
| **Runtime** | .NET | 10.0 |
| **UI Framework** | WPF (Windows Presentation Foundation) | — |
| **UI Toolkit** | WPF-UI (Fluent Design) | 4.3.0 |
| **MVVM Framework** | CommunityToolkit.Mvvm | 8.4.2 |
| **Dependency Injection** | Microsoft.Extensions.Hosting | 10.0.8 |
| **Behaviors** | Microsoft.Xaml.Behaviors.Wpf | 1.1.142 |
| **Language** | C# 13 | — |
| **Platform** | Windows (x64) | — |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- Windows 10 (1809+) or Windows 11
- Visual Studio 2022 17.12+ (recommended) or any IDE with .NET support

### Build & Run

```bash
# Clone the repository
git clone https://github.com/Putra3340/Floatly.git
cd Floatly

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

### Publish (Self-Contained)

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

This produces a single-file, self-contained executable that includes the .NET runtime — no SDK installation required on the target machine.

---

## Project Structure

```
Floatly/
├── App.xaml                          # Application resources & WPF-UI theme dictionaries
├── App.xaml.cs                       # .NET Generic Host setup & DI service registration
├── Usings.cs                         # Global using directives
├── AssemblyInfo.cs                   # Assembly-level theme configuration
├── Floatly.csproj                    # Project file (.NET 10, WPF, NuGet packages)
│
├── Models/
│   ├── AppConfig.cs                  # Application configuration model
│   ├── DataColor.cs                  # Color data model for theming
│   ├── Lyrics.cs                     # Lyrics model with multi-language support
│   ├── MenuItem.cs                   # Navigation menu item model
│   ├── Search.cs                     # Search result model
│   └── Song.cs                       # Song model (title, uploader, thumbnail, video URL)
│
├── ViewModels/
│   ├── Pages/
│   │   ├── HomeViewModel.cs          # Home page logic, song search, playback triggers
│   │   ├── DataViewModel.cs          # Data browser with song listing
│   │   └── SettingsViewModel.cs      # Theme switching, accent color, app version
│   └── Windows/
│       ├── MainWindowViewModel.cs    # Navigation menu, footer controls, tray icon
│       └── FloatingWindowViewModel.cs# PiP playback controls, seek bar, volume, lyrics
│
├── Views/
│   ├── Pages/
│   │   ├── HomePage.xaml/.cs         # Home page UI (search bar, song cards)
│   │   ├── DataPage.xaml/.cs         # Data browser UI (song list)
│   │   └── SettingsPage.xaml/.cs     # Settings UI (theme, accent, about)
│   ├── Windows/
│   │   ├── MainWindow.xaml/.cs       # Main navigation window (INavigationWindow)
│   │   └── FloatingWindow.xaml/.cs   # Floating PiP mini-player window
│   └── Controls/
│       ├── GlobalPlayerControl.xaml/.cs  # Shared MediaElement container (reparented between windows)
│       └── SlideMenu.xaml/.cs        # Animated slide-out lyrics/menu panel
│
├── Services/
│   ├── Interfaces/
│   │   ├── IApiService.cs            # API service contract (search, song details, lyrics)
│   │   └── IPlayerService.cs         # Player service contract (play, pause, seek, volume)
│   ├── ApiService.cs                 # HTTP client for Floatly server API
│   ├── PlayerService.cs              # MediaElement wrapper with SRT parsing & lyric sync
│   └── ApplicationHostService.cs     # App lifecycle (startup window activation)
│
├── Helpers/
│   ├── ThemeHelper.cs                # Accent color application via WPF-UI theming
│   └── EnumToBooleanConverter.cs     # IValueConverter for radio button enum binding
│
├── Enums/
│   └── ThemeAccent.cs                # Accent color enumeration
│
├── Messages/
│   └── SongMessage.cs                # MVVM messenger message for song selection events
│
├── Resources/
│   └── Translations.cs              # Localization placeholder
│
└── Assets/
    ├── wpfui-icon-256.png            # App icon (256px)
    └── wpfui-icon-1024.png           # App icon (1024px)
```

---

## Key Components

### 🪟 MainWindow (`FluentWindow`)

The primary application window implementing `INavigationWindow` from WPF-UI. Built as a `FluentWindow` with **Mica backdrop**, it features:
- Left-side **Fluent navigation pane** (`LeftFluent` mode) with `SymbolRegular` icons
- Page routing to Home, Data, and Settings pages with `BreadcrumbBar` header
- A persistent **bottom playback bar** (90px) showing: song thumbnail (58×58 rounded), title, uploader, progress slider with time labels, transport controls (Rewind 10s / Play-Pause / Skip 10s), and a **PiP button** to open the floating window
- Custom title bar with app icon, extends content into title bar area
- Auto-reparents the `GlobalPlayerControl` on window focus/blur events

### 🎈 FloatingWindow (PiP)

A borderless, transparent, always-on-top (`Topmost=True`) Picture-in-Picture window (640×360). Features:
- **Video playback** via the reparented `GlobalPlayerControl` with `MediaElement`
- **Blurred thumbnail background** using `ImageBrush` on a full-size `Rectangle`
- **Synchronized captions overlay** — centered bottom `TextBlock` with pill-shaped black background
- **Controls overlay** (semi-transparent `#88000000` bar) that appears on window focus and hides on blur
- **SlideMenu** (right side, 210px) for caption language selection
- **Draggable** via `MouseLeftButtonDown` → `DragMove()`
- Transport controls: Rewind 10s, Play/Pause, Skip 10s, seek slider, and Closed Captions button

### 🎶 PlayerService

The centralized playback engine wrapping WPF's `MediaElement`. Key capabilities:
- **Playback**: Play, Pause, Resume, Seek with URI-based source loading
- **Position tracking**: `DispatcherTimer` at 500ms intervals updates position and fires `StateChanged`
- **SRT Parser**: Built-in static `ParseSrt()` method that splits SRT text into `LyricLine` objects with `StartTime`, `EndTime`, and `Text`
- **Lyric Sync**: On each timer tick, finds the matching `LyricLine` for the current position and sets `ActiveLyric`
- **Multi-language captions**: `LoadLyric()` populates available languages, `SelectLyric()` parses the selected SRT
- Exposes state through `IPlayerService` interface for clean DI consumption

### 🌐 ApiService

HTTP client service communicating with the Floatly backend (`https://floatly.starhost.web.id/api/v1/`). Methods:
- `GetSongAsync()` — Fetches the default song list
- `GetSongAsync(query)` — Searches with `anycontent` parameter
- `GetSongDetailAsync(id)` — Gets full song details including `VideoPlaybackUrl`
- `GetCaptionAsync(id)` — Fetches multi-language captions/lyrics
- Uses `HttpClient` with token-based authentication and `System.Net.Http.Json` deserialization

### 📜 SlideMenu

A fully custom WPF `UserControl` with dependency properties and storyboard animation:
- **Dependency Properties**: `IsOpen` (bool), `MenuWidth` (double, default 280), `OverlayOpacity` (double, default 0.5), `ItemsSource` (IEnumerable)
- **Animation**: `DoubleAnimation` on `TranslateTransform.X` with `CubicEase` easing, 250ms duration
- Slides from off-screen (`MenuWidth` offset) to visible (`0`) with overlay dimming
- Items rendered as WPF-UI `Card` containing `ScrollViewer` → `ItemsControl` → `ui:Button` (Primary appearance)

### 🎮 GlobalPlayerControl

A lightweight `UserControl` containing a single `MediaElement` with `LoadedBehavior="Manual"` and `UnloadedBehavior="Manual"`. On load, it resolves `IPlayerService` from DI and calls `Initialize(Player)` to hand the `MediaElement` reference to the service. This control is the **reparenting target** — it gets physically detached and reattached between `MainWindow.PlayerHost` and `FloatingWindow.PlayerHost` `ContentControl` elements.

---

## Design Patterns

| Pattern | Implementation |
|---|---|
| **MVVM** | Views bind to ViewModels via `DataContext`; ViewModels use `[ObservableProperty]` and `[RelayCommand]` source generators from CommunityToolkit.Mvvm |
| **Dependency Injection** | All services, ViewModels, and windows registered in `App.xaml.cs` using `Microsoft.Extensions.Hosting` |
| **Event-Driven State** | `PlayerService.StateChanged` event (500ms timer + action triggers) propagates state to all subscribed ViewModels |
| **MediaElement Reparenting** | Single `GlobalPlayerControl` physically moved between windows for seamless shared playback |
| **Service Abstraction** | All services implement interfaces (`IApiService`, `IPlayerService`) for loose coupling and testability |
| **Hosted Services** | `ApplicationHostService` manages app lifecycle as an `IHostedService` |
| **Navigation Awareness** | ViewModels implement `INavigationAware` for lazy initialization on first navigation |
| **Value Converters** | `EnumToBooleanConverter` for clean enum ↔ radio button binding in settings |
| **Custom Controls** | `SlideMenu` with dependency properties and `DoubleAnimation` storyboard |

---

## Floatly Server

Floatly connects to a backend server for content search, video streaming, and caption retrieval:

| | URL |
|---|---|
| **Official Server** | https://floatly.starhost.web.id |
| **API Base URL** | `https://floatly.starhost.web.id/api/v1/` |
| **Self-Host Repository** | [Floatly-Server](https://github.com/Putra3340/Floatly-Server) |

---

## Contributing

Contributions are welcome! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Commit** your changes with clear messages
   ```bash
   git commit -m "feat: add your feature description"
   ```
4. **Push** to your branch
   ```bash
   git push origin feature/your-feature-name
   ```
5. **Open** a Pull Request

### Code Guidelines

- Follow the **MVVM pattern** — never put business logic in code-behind files
- All new services must implement an **interface** and be registered in the DI container
- Use **CommunityToolkit.Mvvm** attributes (`[ObservableProperty]`, `[RelayCommand]`) for ViewModel properties and commands
- Prefer **event-driven state propagation** (`StateChanged` events) over direct ViewModel coupling
- Maintain the existing **project structure** (Models, ViewModels, Views, Services, Helpers)
- Use `INavigationAware` for ViewModels that need lazy initialization on first navigation

---

## Application Flow

```
1. App.xaml.cs → .NET Generic Host boots → DI container initialized
2. ApplicationHostService.StartAsync() → Resolves MainWindow → Shows it → Navigates to HomePage
3. User searches → HomeViewModel calls ApiService.GetSongAsync() → Results populate ListView
4. User clicks a song → GetSongDetailAsync() gets VideoPlaybackUrl → PlayerService.Play(url)
5. GlobalPlayerControl's MediaElement starts playback → Captions fetched and loaded
6. MainWindow loses focus → GlobalPlayerControl reparented to FloatingWindow → PiP appears
7. MainWindow regains focus → GlobalPlayerControl reparented back → PiP hides
8. User opens SlideMenu in PiP → Selects caption language → SRT parsed → ActiveLyric syncs
9. Settings page → Theme/accent changes applied via ApplicationThemeManager
```

---

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright © 2026 Putra3340

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## Acknowledgements

- [WPF-UI](https://github.com/lepoco/wpfui) — Fluent Design System for WPF
- [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) — Modern MVVM toolkit
- [Microsoft.Extensions.Hosting](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host) — .NET Generic Host
- .NET Community & Open Source Contributors

---

<p align="center">
  <sub>Built with ❤️ by <a href="https://github.com/Putra3340">Putra3340</a></sub>
</p>
