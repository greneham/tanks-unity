using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action<int> onMoveTurnStart;
    public void MoveTurnStart(int id)
    {
        if(onMoveTurnStart != null)
        {
            onMoveTurnStart(id);
        }
    }

    public event Action<int, Vector3> onMoveTurnEnd;
    public void MoveTurnEnd(int id, Vector3 pos)
    {
        if (onMoveTurnEnd != null)
        {
            onMoveTurnEnd(id, pos);
        }
    }

    public event Action<CameraMode> onCameraModeChangeStart;
    public void CameraModeChangeStart(CameraMode id)
    {
        if (onCameraModeChangeStart != null)
        {
            onCameraModeChangeStart(id);
        }
    }

    public event Action<CameraMode> onCameraModeChangeEnd;
    public void CameraModeChangeEnd(CameraMode id)
    {
        if (onCameraModeChangeEnd != null)
        {
            onCameraModeChangeEnd(id);
        }
    }

    public event Action<int> onSetTankPositionStart;
    public void SetTankPositionStart(int id)
    {
        if (onSetTankPositionStart != null)
        {
            onSetTankPositionStart(id);
        }
    }

    public event Action<int> onSetTankPositionEnd;
    public void SetTankPositionEnd(int id)
    {
        if (onSetTankPositionEnd != null)
        {
            onSetTankPositionEnd(id);
        }
    }

    public event Action<int> onTankMoveStart;
    public void TankMoveStart(int id)
    {
        if (onTankMoveStart != null)
        {
            onTankMoveStart(id);
        }
    }

    public event Action<int> onTankMoveEnd;
    public void TankMoveEnd(int id)
    {
        if (onTankMoveEnd != null)
        {
            onTankMoveEnd(id);
        }
    }

    public event Action<int> onTankReadyFireStart;
    public void TankReadyFireStart(int id)
    {
        if (onTankReadyFireStart != null)
        {
            onTankReadyFireStart(id);
        }
    }

    public event Action<int> onTankReadyFireEnd;
    public void TankReadyFireEnd(int id)
    {
        if (onTankReadyFireEnd != null)
        {
            onTankReadyFireEnd(id);
        }
    }


    public event Action<int> onTankFireStart;
    public void TankFireStart(int id)
    {
        if (onTankFireStart != null)
        {
            onTankFireStart(id);
        }
    }

    public event Action<int> onTankFireEnd;
    public void TankFireEnd(int id)
    {
        if (onTankFireEnd != null)
        {
            onTankFireEnd(id);
        }
    }

    public event Action<Vector3, ExplosionType, int> onExplosionAt;
    public void ExplosionAt(Vector3 pos, ExplosionType explosionType, int id)
    {
        if (onExplosionAt != null)
        {
            onExplosionAt(pos, explosionType, id);
        }
    }

    public event Action<bool> onGameOver;
    public void GameOver(bool youWin)
    {
        if (onGameOver != null)
        {
            onGameOver(youWin);
        }
    }
}
