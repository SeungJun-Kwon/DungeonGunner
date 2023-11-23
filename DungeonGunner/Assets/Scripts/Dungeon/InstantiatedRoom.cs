using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room _room;
    [HideInInspector] public Grid _grid;
    [HideInInspector] public Tilemap _groundTilemap;
    [HideInInspector] public Tilemap _decoration1Tilemap;
    [HideInInspector] public Tilemap _decoration2Tilemap;
    [HideInInspector] public Tilemap _frontTilemap;
    [HideInInspector] public Tilemap _collisionTilemap;
    [HideInInspector] public Tilemap _minimapTilemap;
    [HideInInspector] public Bounds _roomColliderBounds;

    BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        TryGetComponent(out _boxCollider2D);

        _roomColliderBounds = _boxCollider2D.bounds;
    }

    public void Initialise(GameObject roomGameObject)
    {
        PopulateTilemapMemberVariables(roomGameObject);

        BlockOffUnusedDoorways();

        DisableCollisionTilemapRenderer();
    }

    void PopulateTilemapMemberVariables(GameObject roomGameObject)
    {
        _grid = roomGameObject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        foreach(Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.tag == "groundTilemap")
            {
                _groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                _decoration1Tilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "decoration2Tilemap")
            {
                _decoration2Tilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "frontTilemap")
            {
                _frontTilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "collisionTilemap")
            {
                _collisionTilemap = tilemap;
            }
            else if(tilemap.gameObject.tag == "minimapTilemap")
            {
                _minimapTilemap = tilemap;
            }
        }
    }

    void BlockOffUnusedDoorways()
    {
        foreach(var doorway in _room._doorwayList)
        {
            if (doorway.isConnected)
                continue;

            if(_groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(_groundTilemap, doorway);
            }

            if (_decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(_decoration1Tilemap, doorway);
            }

            if (_decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(_decoration2Tilemap, doorway);
            }

            if (_frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(_frontTilemap, doorway);
            }

            if (_collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(_collisionTilemap, doorway);
            }

            if (_minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(_minimapTilemap, doorway);
            }
        }
    }

    void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            default:
                break;
        }
    }

    void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for(int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for(int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y + yPos, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos,
                    startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos,
                    startPosition.y - yPos, 0)));

                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);
            }
        }
    }

    void DisableCollisionTilemapRenderer()
    {
        _collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}
