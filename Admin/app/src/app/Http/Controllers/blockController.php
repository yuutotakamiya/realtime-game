<?php

namespace App\Http\Controllers;

use App\Models\block;
use Illuminate\Http\Request;

class blockController extends Controller
{
    //
    public function index(Request $request)
    {
        $block = block::all();

        return view('block.block', ['block' => $block]);
    }
}
