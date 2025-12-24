# Floatly

**Floatly** is a modern, lightweight, and customizable music player application designed for seamless audio playback on Windows. Built with performance and aesthetics in mind, it features a unique floating "always-on-top" interface that allows you to control your music without interrupting your workflow.

Powered by **.NET 10** and **WPF**, Floatly combines cutting-edge technology with a sleek, minimalist design.

---

## Features

- **Floating Interface**: An unobtrusive mini-player that stays on top of other windows.
- **Immersive Fullscreen**: Switch to a rich, fullscreen experience when you want to focus on the music.
- **Lyrics Integration**: Display synchronized lyrics in a dedicated floating window.
- **Universal Playback**: Support for both local music files and online streams.
- **Advanced Audio Control**: Built-in 10-band equalizer for personalized sound.
- **User System**: Secure Login and Registration integrated with Floatly Server.
- **Library Management**: Organize your playlists and favorites efficiently.
- **Global Shortcuts**: Control playback from anywhere using keyboard hotkeys.

---

## Technology Stack

Floatly is built using the latest Microsoft technologies:

- **Framework**: .NET 10
- **UI Framework**: Windows Presentation Foundation (WPF)
- **Data Access**: Entity Framework Core
- **Database**: SQLite / SQL Server support
- **Audio Metadata**: TagLibSharp
- **Input Handling**: MouseKeyHook for global hotkeys

---

## Project Structure

The project is organized into clear architectural layers:

- **`Api/`**: Handles communication with the remote Floatly Server/Backend.
- **`Forms/`**: Contains all UI windows and views (e.g., `FloatingWindow`, `FullScreenWindow`, `EqualizerWindow`).
- **`Models/`**: Defines data structures.
    - `Database/`: Entity Framework entities.
    - `Form/`: View-specific models for UI binding.
- **`Utils/`**: Helper classes and extension methods.

---

## Code Style

To maintain codebase quality and stability, strictly adhere to the following guidelines:

1.  **Encapsulation Over Direct Access**
    - Avoid accessing `MainWindow.instance` directly.
    - Use exposed properties or dedicated methods on `MainWindow` to interact with the core logic.

2.  **Modern .NET Standards**
    - Utilize **.NET 10** features and syntax enhancements wherever possible to ensure future-proofing and performance.

3.  **Strict Model Separation**
    - **Never** bind UI elements directly to API or Database models.
    - **Always** separate the data layer from the presentation layer by using dedicated **Form Models**.
    - *Reference*: See [PlayerCardModel.cs](Models/Form/PlayerCardModel.cs) and [StaticBinding.cs](Models/Form/StaticBinding.cs) for correct implementation patterns.

---

## Contributing

Contributions are welcome! Please read the [contributing guidelines](CONTRIBUTING.md) before submitting a pull request.

1.  Fork the repository.
2.  Create a new branch (`git checkout -b feature/your-feature`).
3.  Commit your changes (`git commit -am 'Add new feature'`).
4.  Push to the branch (`git push origin feature/your-feature`).
5.  Open a pull request.

---

## Floatly Server

Floatly connects to a centralized server for user management and synchronization:

- **Official Server**: https://floatly.starhost.web.id
- **Self-Host**: [Floatly Self-Host Server Repository](https://github.com/Putra3340/Floatly-Server)

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

**Copyright © 2025 Putra3340.**

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

---

## Acknowledgements

- .NET Community
- Open Source Contributors

---

## Contact

For questions, issues, or feature requests, please open an issue on GitHub or contact [masputrabro@gmail.com](mailto:masputrabro@gmail.com).
