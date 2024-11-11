<?php

namespace App\Http\Controllers;

use App\Models\Stage;
use App\Models\StageLog;
use Database\Seeders\StageTableSeeder;
use Illuminate\Http\Request;


class stageController extends Controller
{
    public function index()
    {
        $stage = Stage::all();

        return view('stage.stage', ['stage' => $stage]);

    }

    public function show_stage_log(Request $request)
    {
        $stage = Stage::find($request->id);

        if (!empty($stage)) {
            $logs = $stage->stage_logs()->paginate(10);
            $logs->appends(['id' => $request->id]);
        }

        return view('stage.stage_log', ['stage_log' => $logs ?? null]);
    }
}
