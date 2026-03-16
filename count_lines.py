#!/usr/bin/env python3
"""
Counts the total number of lines in all .cs files in the src/GoogolSharp directory.
"""

from pathlib import Path

def count_lines_in_project():
    src_path = Path("src/GoogolSharp")
    
    if not src_path.exists():
        print(f"Error: {src_path} directory not found")
        return
    
    total_lines = 0
    file_count = 0
    
    # Find all .cs files recursively
    cs_files = sorted(src_path.rglob("*.cs"))
    
    if not cs_files:
        print("No .cs files found")
        return
    
    for cs_file in cs_files:
        try:
            with open(cs_file, 'r', encoding='utf-8') as f:
                lines = len(f.readlines())
                total_lines += lines
                file_count += 1
                print(f"{cs_file}: {lines} lines")
        except Exception as e:
            print(f"Error reading {cs_file}: {e}")
    
    print(f"\n{'='*50}")
    print(f"Total: {total_lines} lines in {file_count} files")
    print(f"{'='*50}")

if __name__ == "__main__":
    count_lines_in_project()
