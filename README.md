# Floatly

Floatly is a modern, lightweight, and customizable music player application designed for seamless audio playback on Windows. The project emphasizes a floating, always-on-top user interface, allowing users to control their music without interrupting their workflow. Built with performance and usability in mind, Floatly leverages the latest .NET technologies and follows best practices in software development.

---

## Features
- Play your favorite local/online music files.
- Display synchronized lyrics in a floating window.
- Minimalistic and user-friendly interface.

---

## Contributing

Contributions are welcome! Please read the [contributing guidelines](CONTRIBUTING.md) before submitting a pull request.

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -am 'Add new feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a pull request.

---

## Floatly Server
- https://floatly.putrartx.my.id
- [Floatly Self-Host Server](https://github.com/Putra3340/Floatly-Server)


## Code Style
- Minimize direct Mainwindow.instance use Mainwindow.variable or Mainwindow.Method() instead.
- .NET 10 features must be used where possible.
- Never use Api/Database Model directly to bind/set datacontext UI elements to Always use Form Model (see [PlayerCardModel.cs](Models/Form/PlayerCardModel.cs) and [StaticBindingModel.cs](Models/Form/StaticBindingModel.cs)).

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
