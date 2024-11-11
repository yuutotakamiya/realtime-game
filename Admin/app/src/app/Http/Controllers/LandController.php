<?php

namespace App\Http\Controllers;

use App\Models\Land;
use App\Models\Landstatus;
use Illuminate\Http\Request;

class LandController extends Controller
{
    //島の一覧情報をviewに渡す
    public function index()
    {
        $land = Land::all();

        return view('land.land', ['land' => $land]);
    }

    public function show_land_status(Request $request)
    {
        $landstatus = Landstatus::all();
        return view('land.landstatus', ['landstatus' => $landstatus]);
    }
}
