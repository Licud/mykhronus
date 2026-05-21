# MyKhronus

A little WPF app for tracking where your time actually goes during the day.

App just runs on your machine, saves to a local DB, and stays out of the way.

## What it does

- **My Day tab** — add work items, hit play, do the thing, hit pause. Switch between tasks, the timer follows you. Pick a different date if you forgot to log something yesterday.
- **Reports tab** — pick a date range, get a per-work-item breakdown with each day as a tidy little pill. Copy duration, copy description, hide rows you don't care about.
- **Autosave** — saves time tracked every 3 minutes. Manual save button is right there too.
- **Global play/pause** — one set of controls drives whichever timer is currently active, even when you're scrolling through past dates.

## Future

v1.2.3
- Small bug fixes

v1.3.0
- Implementing a scheduled work items - a sticky list that a user can populate to determine tasks they still need to do. Might update this one to be called ToDo list now that I think about it.
- Add some unit tests to the viewmodels - yeah, should've done tdd but if I'm honest, didn't think the app was going to be as useful to me as it is now so adding in unit tests would be a plan considering I plan to add more functionality to make my time management simpler
