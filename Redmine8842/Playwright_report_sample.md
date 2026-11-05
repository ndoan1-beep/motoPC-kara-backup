→ browser_navigate { url: "https://demo.playwright.dev/todomvc" }

  - heading "todos" [level=1] [ref=e3]
  - textbox "What needs to be done?" [ref=e5]

→ browser_type { target: "e5", text: "Buy groceries", submit: true }

  - heading "todos" [level=1] [ref=e3]
  - textbox "What needs to be done?" [ref=e5]
  - list [ref=e8]:
    - listitem [ref=e9]:
      - checkbox "Toggle Todo" [ref=e10]
      - text: Buy groceries
  - contentinfo [ref=e12]:
    - text: 1 item left