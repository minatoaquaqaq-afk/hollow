from pathlib import Path
from collections import deque
from PIL import Image
import sys

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "Assets" / "Art" / "UserProvided" / "ArcadeDistrict"
SOURCE = ART / "Source"
COMPONENTS = ART / "Components"
PREVIEWS = ART / "Previews"

REFERENCE = SOURCE / "reference.png"
BACKGROUND = SOURCE / "background.png"
PROPS1 = SOURCE / "props_1.png"
PROPS2 = SOURCE / "props_2.png"


def require(path: Path) -> None:
    if not path.exists():
        raise FileNotFoundError(f"Missing required source image: {path}")


def save_copy(src: Path, dst: Path) -> None:
    img = Image.open(src).convert("RGBA")
    img.save(dst)


def trim_alpha(img: Image.Image) -> Image.Image:
    bbox = img.getchannel("A").getbbox()
    return img.crop(bbox) if bbox else img


def dark_bg_to_alpha(img: Image.Image, threshold: int = 70) -> Image.Image:
    img = img.convert("RGBA")
    px = img.load()
    w, h = img.size
    seen = set()
    q = deque()
    for x in range(w):
        q.append((x, 0))
        q.append((x, h - 1))
    for y in range(h):
        q.append((0, y))
        q.append((w - 1, y))

    while q:
        x, y = q.popleft()
        if (x, y) in seen or not (0 <= x < w and 0 <= y < h):
            continue
        seen.add((x, y))
        r, g, b, a = px[x, y]
        if max(r, g, b) > threshold:
            continue
        px[x, y] = (r, g, b, 0)
        q.extend(((x + 1, y), (x - 1, y), (x, y + 1), (x, y - 1)))
    return trim_alpha(img)


def crop(src: Path, name: str, box: tuple[int, int, int, int], bg_threshold: int = 18) -> None:
    img = Image.open(src).convert("RGBA").crop(box)
    img = dark_bg_to_alpha(img, bg_threshold)
    img.save(COMPONENTS / f"{name}.png")


def build_components() -> None:
    COMPONENTS.mkdir(parents=True, exist_ok=True)
    # props_1.png: 1680x945-ish sheet with signs, arcade machines, pedestals, boxes and decor.
    crop(PROPS1, "insert_coin_sign", (45, 35, 345, 245))
    crop(PROPS1, "game_over_sign", (375, 45, 660, 235))
    crop(PROPS1, "arcade_sign", (700, 55, 960, 205))
    crop(PROPS1, "neon_nights_sign", (1010, 35, 1190, 255))
    crop(PROPS1, "power_up_sign", (1230, 45, 1370, 260))
    crop(PROPS1, "power_pedestal_blue", (1390, 50, 1485, 260))
    crop(PROPS1, "power_pedestal_magenta", (1505, 50, 1605, 260))
    crop(PROPS1, "arcade_machine_a", (40, 280, 145, 455))
    crop(PROPS1, "arcade_machine_b", (165, 280, 270, 455))
    crop(PROPS1, "arcade_machine_c", (290, 280, 395, 455))
    crop(PROPS1, "terminal_large", (50, 480, 315, 610))
    crop(PROPS1, "shop_stall", (345, 480, 615, 610))
    crop(PROPS1, "bench_barrier", (620, 780, 920, 875))
    crop(PROPS1, "chest_orange", (895, 475, 1000, 560))
    crop(PROPS1, "chest_blue", (1245, 430, 1345, 545))
    crop(PROPS1, "crate_magenta", (1455, 430, 1555, 545))
    crop(PROPS1, "plant_large", (250, 620, 355, 745))
    crop(PROPS1, "vent_double", (1085, 620, 1280, 740))
    crop(PROPS1, "floor_arrow_up", (995, 790, 1080, 875))
    crop(PROPS1, "barrel_group", (1095, 285, 1290, 405))

    # props_2.png: tile/wall/gate module sheet.
    crop(PROPS2, "floor_tile_blue", (25, 50, 120, 140))
    crop(PROPS2, "floor_tile_wet_blue", (25, 150, 120, 245))
    crop(PROPS2, "floor_tile_wet_magenta", (230, 150, 320, 245))
    crop(PROPS2, "north_gate", (920, 92, 1165, 235))
    crop(PROPS2, "south_gate", (1180, 92, 1425, 235))
    crop(PROPS2, "west_gate", (915, 280, 1155, 415))
    crop(PROPS2, "east_gate", (1180, 280, 1425, 415))
    crop(PROPS2, "door_frame_wide", (1410, 55, 1615, 190))
    crop(PROPS2, "wall_segment_long", (350, 50, 640, 150))
    crop(PROPS2, "corner_left", (675, 50, 765, 185))
    crop(PROPS2, "corner_right", (795, 50, 890, 185))
    crop(PROPS2, "power_pedestal_1", (680, 405, 760, 530))
    crop(PROPS2, "power_pedestal_2", (775, 405, 855, 530))
    crop(PROPS2, "terminal_block_a", (940, 405, 1015, 520))
    crop(PROPS2, "terminal_block_b", (1025, 405, 1105, 520))
    crop(PROPS2, "cover_piece_small", (1285, 405, 1360, 505))
    crop(PROPS2, "vent_grate", (595, 610, 675, 695))


def compose_preview() -> None:
    PREVIEWS.mkdir(parents=True, exist_ok=True)
    bg = Image.open(ART / "arcade_background.png").convert("RGBA").resize((1600, 900), Image.LANCZOS)
    canvas = bg.copy()
    placements = {
        "shop_stall": [(560, 455, 230)],
        "terminal_large": [(1010, 455, 210)],
        "power_pedestal_blue": [(760, 290, 95)],
        "power_pedestal_magenta": [(760, 575, 95), (1230, 410, 95)],
        "arcade_machine_a": [(1160, 600, 90)],
        "arcade_machine_b": [(1230, 600, 90)],
        "arcade_machine_c": [(1300, 600, 90)],
        "power_up_sign": [(1260, 320, 120)],
        "chest_orange": [(430, 515, 85)],
        "floor_arrow_up": [(1110, 625, 80)],
        "barrel_group": [(1020, 360, 100)],
        "insert_coin_sign": [(565, 155, 190)],
        "game_over_sign": [(1135, 155, 190)],
        "arcade_sign": [(1345, 190, 155)],
        "neon_nights_sign": [(210, 690, 120)],
    }
    for name, plist in placements.items():
        path = COMPONENTS / f"{name}.png"
        if not path.exists():
            continue
        sprite = Image.open(path).convert("RGBA")
        for cx, cy, target_w in plist:
            scale = target_w / max(1, sprite.width)
            resized = sprite.resize((max(1, int(sprite.width * scale)), max(1, int(sprite.height * scale))), Image.LANCZOS)
            canvas.alpha_composite(resized, (int(cx - resized.width / 2), int(cy - resized.height / 2)))
    canvas.save(PREVIEWS / "layered_testroom_preview.png")


def main() -> int:
    for path in (REFERENCE, BACKGROUND, PROPS1, PROPS2):
        require(path)
    ART.mkdir(parents=True, exist_ok=True)
    save_copy(REFERENCE, ART / "reference_overlay.png")
    save_copy(BACKGROUND, ART / "arcade_background.png")
    build_components()
    compose_preview()
    print("Built user arcade art:")
    print(f"  {ART / 'arcade_background.png'}")
    print(f"  {COMPONENTS}")
    print(f"  {PREVIEWS / 'layered_testroom_preview.png'}")
    return 0


if __name__ == "__main__":
    try:
        raise SystemExit(main())
    except Exception as exc:
        print(exc, file=sys.stderr)
        raise SystemExit(1)
