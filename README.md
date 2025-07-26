# Floatly

Floatly is a modern, lightweight, and customizable music player application designed for seamless audio playback on Windows. The project emphasizes a floating, always-on-top user interface, allowing users to control their music without interrupting their workflow. Built with performance and usability in mind, Floatly leverages the latest .NET technologies and follows best practices in software development.

---

## Features

- **Floating UI:** Always-on-top, minimalistic music player window.
- **Wide Audio Format Support:** Plays MP3, WAV, FLAC, AAC, and more.
- **Playlist Management:** Create, edit, and save playlists.
- **Media Controls:** Play, pause, stop, next, previous, seek, and volume control.
- **Drag-and-Drop:** Easily add music files or folders.
- **System Tray Integration:** Minimize to tray and control playback from the tray.
- **Keyboard Shortcuts:** Global hotkeys for quick control.
- **Lightweight:** Low memory and CPU usage.
- **Customizable Appearance:** Theme and color options.

---

## Getting Started

### Prerequisites

- Windows 10 or later
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or newer

### Installation

1. **Clone the repository:**
    ```bash
    git clone https://github.com/Putra3340/Floatly
    cd Floatly
    ```

2. **Build the project:**
    ```bash
    dotnet build
    ```

3. **Run the application:**
    ```bash
    dotnet run
    ```

### Usage

- Launch the application.
- Use the floating controls to manage playback.
- Access settings to customize appearance and behavior.

---

## Project Structure

```
/Floatly
│
├── Floatly/           # Main application source code
│   ├── Models/              # Data models (tracks, playlists, etc.)
│   ├── Views/               # UI components and windows
│   ├── ViewModels/          # MVVM view models
│   ├── Services/            # Audio playback, file handling, etc.
│   └── App.xaml, MainWindow.xaml
│
├── README.md
└── LICENSE
```

---

## Contributing

Contributions are welcome! Please read the [contributing guidelines](CONTRIBUTING.md) before submitting a pull request.

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a pull request.

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
