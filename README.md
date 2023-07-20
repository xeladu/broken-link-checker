# Broken Link Checker
This simple command-line tool checks all external links of a website if they are reachable. It's a .NET7 Console Application and works with Windows, Linux, and MacOS.

# How does it work?
The tool parses the given URL for links and collects them. In the next step, it performs an HTTP HEAD request to all found links and collects the results. A final summary indicates which links are broken.

# How to run?
![image](https://user-images.githubusercontent.com/97696030/216458787-e9dc5626-8605-4f2d-b0cb-51bddd7b8db9.png)

# Examples
![image](https://user-images.githubusercontent.com/97696030/216459592-4ead468e-302d-4220-9e3c-6695f3d433fc.png)

# Limitations
- Some websites try to prevent requests from anything than a browser
- A broken doesn't necessarily mean that the website is down. When in doubt, double-check with a browser

# About me

- Follow me on [Medium](https://xeladu.medium.com)
- Visit my [QuickCoder blog](https://quickcoder.org)
- Check out my [digital products](https://xeladu.gumroad.com)
