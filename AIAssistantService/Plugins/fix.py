import os
import json

with open("generate_plugin.py", "r", encoding="utf-8") as f:
    content = f.read()

content = content.replace("SafeGetAsync($\\\"{{{path_url}}}\\\" + qStr)", "SafeGetAsync($\\\"{path_url}\\\" + qStr)")
content = content.replace("SafePostAsync($\\\"{{{path_url}}}\\\" + qStr, {post_body})", "SafePostAsync($\\\"{path_url}\\\" + qStr, {post_body})")

with open("generate_plugin.py", "w", encoding="utf-8") as f:
    f.write(content)
