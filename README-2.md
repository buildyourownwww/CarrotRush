# Portal Run

## About the Game

Portal Run is a 2D game developed in Unity. The player explores levels,
collects carrots, avoids enemies, and tries to complete each level.

## Audio Implementation

The game uses a custom Audio Mixer with three groups:

-   **Master** --- controls the overall game volume.
-   **Music** --- controls background music.
-   **SFX** --- controls sound effects.

### Background Music

The background music tracks were downloaded from **Pixabay**.

  ---------------------------------------------------------------------------------------------------
  Track          Usage          Source Website                          Asset Link     License
  -------------- -------------- --------------------------------------- -------------- --------------
  Menu Music     Main Menu      [Pixabay](https://pixabay.com)   Add the direct Pixaba                                                                     
https://pixabay.com/music/video-games-retro-arcade-game-music-629809/                                                                                    Pixabay Content License

  Level Music    Gameplay       [Pixabay](https://pixabay.com)   Add the direct Pixabay
https://pixabay.com/music/video-games-retro-game-593063/                                                  track page URL Content
Pixabay Content License                                                                               
  ---------------------------------------------------------------------------------------------------

### Sound Effects

The sound effects were downloaded from **Pixabay**.

  -----------------------------------------------------------------------------------------------------------
  Sound Effect   Usage          Source Website                                  Asset Link     License
  -------------- -------------- ----------------------------------------------- -------------- --------------
  Jump           Player jumping [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
                                                                                sound page https://pixabay.com/sound-effects/film-special-effects-cartoon-jump-463196/
                                                                                               Pixabay Content License

  Hit            Player taking  [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
                 damage                                                         sound page URL Content
                                                                                               https://pixabay.com/sound-effects/film-special-effects-bulletimpact2-442718/

  Death          Player death   [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
                                                                                sound page URL Content
                                                                                               https://pixabay.com/sound-effects/film-special-effects-character-fall-impact-352287/

  Carrot Pickup  Collecting     [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
                 carrots                                                        sound page URL Content
                                                                                               https://pixabay.com/sound-effects/film-special-effects-retro-arcade-coin-pickup-622953/

  Charger        Charger        [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
  Wind-up        preparing an                                                   sound page URL Content
                 attack                                                                        https://pixabay.com/sound-effects/film-special-effects-062708-laser-charging-81968/

  Shooter Fire   Shooter firing [Pixabay](https://pixabay.com/sound-effects/)   
                 shooting                                                        https://pixabay.com/sound-effects/film-special-effects-shoot-1-81135/
                                                                                               Pixabay Content License

  UI Click       Button         [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
                 interactions                                                   sound page https://pixabay.com/sound-effects/film-special-effects-button-click-vintage-sound-fx-541135/
                                                                                               Pixabay Content License

  Win            Completing a   [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
                 level                                                          sound page https://pixabay.com/sound-effects/gaming-victory-464016/
                                                                                               Pixabay Content License

  Lose           Losing the     [Pixabay](https://pixabay.com/sound-effects/)   Add the direct Pixabay
                 https://pixabay.com/sound-effects/film-special-effects-8-bit-lose-life-634767/ page URL Content
Pixabay Content License                                                                                        License
  -----------------------------------------------------------------------------------------------------------

## Audio Features

-   Separate Music and SFX volume controls.
-   Volume settings are saved using the game's save system.
-   Random pitch variation for selected sound effects.
-   Separate menu and gameplay music tracks.
-   Music crossfade between scenes.
-   Audio Mixer snapshots for normal and paused states.

## Development

-   **Engine:** Unity
-   **Programming Language:** C#
-   **Platform:** iOS

## Credits and Licenses

Audio assets listed above were sourced from Pixabay and are subject to
the [Pixabay Content License](https://pixabay.com/service/license/).

Before submission, replace each asset-link placeholder with the direct
Pixabay page for the exact track or sound effect used in the game. Check
each asset's page to confirm its creator and license details.
