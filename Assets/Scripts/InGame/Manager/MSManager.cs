using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using SimpleActions;
using Type;
using Type.Utils;
using UnityEngine;

public class MSManager : Managers<MSManager>
{
    private MSGenerator generator;
    private MSStartingSelecter starter;
    private MSVerifier verifier;

    public MSConfig mSConfig = new MSConfig();


    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private CellCleaner cellCleaner;

    public SimpleEvent OnFail { get; private set; } = new SimpleEvent();
    public SimpleEvent OnClear { get; private set; } = new SimpleEvent();
    public SimpleEvent<Vector2Byte, Vector2Byte, int> OnStart { get; private set; } = new SimpleEvent<Vector2Byte, Vector2Byte, int>();
    public SimpleEvent<GameObject> OnInstantiate { get; private set; } = new SimpleEvent<GameObject>();

    Coroutine genCoroutine;

    private void Awake()
    {
        Singleton(false);

        generator = new MSGenerator();
        starter = new MSStartingSelecter();
        verifier = new MSVerifier();
    }
    public void MapMake()
    {
        if (genCoroutine != null) return;
        cellCleaner.Clean();

        this.SafeStartCoroutine(ref genCoroutine, MakeStage(mSConfig.seed, mSConfig.diff, mSConfig.size));
    }


    IEnumerator MakeStage(int seed, byte diff, Vector2Byte size)
    {
        sbyte[,] mapData = generator.CreatePuzzle(seed, diff, size);

        Vector2Byte startingPoint = starter.FindStartingPosition(mapData);

        var (verifiedMap, BV) = verifier.Verification(mapData, startingPoint);

        float startX = -size.x / 2f + 0.5f;
        float startY = -size.y / 2f + 0.5f;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3 spawnPosition = new Vector3(startX + x, startY + y, 0);

                GameObject obj = Instantiate(cellPrefab, spawnPosition, Quaternion.identity);

                obj.GetComponent<MSCell>().Bind(verifiedMap[y, x]);

                OnInstantiate.Invoke(obj);
            }

            yield return null;
        }

        OnStart.Invoke(size, startingPoint, BV);

        this.SafeStopCoroutine(ref genCoroutine);
    }


}
