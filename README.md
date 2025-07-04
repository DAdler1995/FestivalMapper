# FestivalMapper

A cross-platform .NET 9 MAUI Blazor app for planning and managing music festival stage layouts, artist lineups, and schedules — all stored locally with no network dependency.

---

## 🚀 Features

- 📸 Upload a custom festival map image (stored as base64)
- 📍 Tap to place stages directly on the map
- 📝 Edit stage names, descriptions, and artist lineups
- ⏱️ Schedule artist set times with automatic conflict detection
- ⚠️ Highlights overlapping set times across all stages
- 💾 Save and load maps locally as `.festivalmap` (json file) files (fully offline)
- 📂 Import existing `.festivalmap` (json file) map files from anywhere on your device

---

## 📁 File Storage

- Saved maps are stored as `.festivalmap` (json file) files in:
  - **Windows/macOS/Android/iOS**: AppData directory under `/maps/`
- Each map includes:
  - Festival name and description
  - Base64-encoded image
  - Stage locations and names
  - Artist lineups and times (stored in UTC)

---

## 🧰 Technologies Used

- **.NET 9**
- **MAUI Blazor Hybrid**
- **Bootstrap 5.1** for responsive styling
- **System.Text.Json** for local serialization
- **MAUI FilePicker & FileSystem APIs**

---

## 🔧 How to Build & Run

1. **Clone the repo**
   ```bash
   git clone https://github.com/DAdler1995/FestivalMapper.git

---

## License

[GNU GPLv3](https://choosealicense.com/licenses/gpl-3.0/)
